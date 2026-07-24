// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Reflection;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Support;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public sealed class FlowInstance
{
    private readonly LogEvent log;

    public FlowInstance(LogEvent log)
    {
        this.log = log;
        Script = new ScriptRunner(log);
    }

    public Guid Id { get; private set; }

    public int AppId { get; private set; }

    public string Caller { get; internal set; }

    public string Name { get; private set; }

    public Guid FlowDefinitionId { get; private set; }

    public Flow Flow { get; private set; }

    public WorkflowExecutionContext Context { get; private set; }

    public DateTimeOffset Start { get; private set; }

    internal IScriptProcessingService Script { get; }

    public async Task<FlowInstanceData> ExecuteAsync(WorkflowRequest request)
    {
        ArgumentNullException.ThrowIfNull(argument: request);

        Start = DateTimeOffset.UtcNow;

        using HttpClient api = CreateApiClient(apiRoot: request.Api);
        api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.AuthToken);

        string rawInstance = await api.GetStringAsync(requestUri: $"Workflow/FlowInstanceData({request.InstanceId})?$expand=FlowDefinition($expand=App)");
        FlowInstanceData instanceData = await DeserializeInstanceAsync(rawInstance: rawInstance);

        AppId = instanceData.FlowDefinition.AppId;
        Id = instanceData.Id;
        Name = instanceData.Name;
        Caller = instanceData.Caller;
        FlowDefinitionId = instanceData.FlowDefinitionId;

        WorkflowContext dtoContext = await DeserializeContextAsync(rawContext: instanceData.ContextString);
        Flow = dtoContext.Flow ?? throw new InvalidOperationException("Flow instance context did not contain a workflow.");

        await StitchAsync();

        Context = new WorkflowExecutionContext(Flow, this);
        await Context.ExecuteAsync(apiRoot: request.Api, authToken: request.AuthToken);

        return Complete();
    }

    internal HttpClient CreateApiClient(string apiRoot) => new(new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        ServerCertificateCustomValidationCallback = CertChainValidator.ValidateCertChain
    })
    {
        BaseAddress = new Uri(apiRoot)
    };

    internal Task LogAsync(WorkflowLogLevel level, string message) => log(level: level, message: message);

    private async Task<FlowInstanceData> DeserializeInstanceAsync(string rawInstance)
    {
        try
        {
            return JsonConvert.DeserializeObject<FlowInstanceData>(value: rawInstance, settings: WorkflowJson.GetJsonSettings())
                ?? throw new InvalidOperationException("Workflow instance response was empty.");
        }
        catch
        {
            await LogAsync(level: WorkflowLogLevel.Error, message: $"Failed to deserialize flow instance:{Environment.NewLine}{rawInstance}");
            throw;
        }
    }

    private async Task<WorkflowContext> DeserializeContextAsync(string rawContext)
    {
        try
        {
            return JsonConvert.DeserializeObject<WorkflowContext>(value: rawContext, settings: WorkflowJson.GetJsonSettings())
                ?? throw new InvalidOperationException("Workflow context response was empty.");
        }
        catch
        {
            await LogAsync(level: WorkflowLogLevel.Error, message: $"Failed to deserialize flow context:{Environment.NewLine}{rawContext}");
            throw;
        }
    }

    private FlowInstanceData Complete()
    {
        foreach (Activity activity in Context.Flow.Activities)
        {
            PropertyInfo[] properties = activity.GetType()
                .GetProperties()
                .Where(predicate: property => property.GetCustomAttribute<IgnoreWhenFlowCompleteAttribute>() is not null)
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                property.SetValue(obj: activity, value: default);
            }
        }

        return new FlowInstanceData
        {
            Id = Id,
            Name = Name,
            Caller = Caller,
            FlowDefinitionId = FlowDefinitionId,
            ContextString = JsonConvert.SerializeObject(value: Context, settings: WorkflowJson.GetJsonSettings()),
            State = Context.ExecutionState,
            Start = Start,
            End = DateTimeOffset.UtcNow
        };
    }

    private async Task StitchAsync()
    {
        foreach (Activity activity in Flow.Activities)
        {
            try
            {
                string[] links = Flow.Links
                    .Where(predicate: link => link.Destination == activity.Ref)
                    .Select(selector: link => link.Source)
                    .ToArray();

                activity.Previous = Flow.Activities.Where(predicate: candidate => links.Contains(value: candidate.Ref)).ToArray();
            }
            catch (Exception exception)
            {
                await LogAsync(
level: WorkflowLogLevel.Error,
message: $"Problem in previous activity selection for activity {activity.Ref}:{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        foreach (Activity activity in Flow.Activities)
        {
            try
            {
                activity.Next = Flow.Activities.Where(predicate: candidate => candidate.Previous?.Contains(value: activity) ?? false).ToArray();
            }
            catch (Exception exception)
            {
                await LogAsync(
level: WorkflowLogLevel.Error,
message: $"Problem in next activity selection for activity {activity.Ref}:{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }

            try
            {
                activity.AssignCode = BuildAssign(activity: activity, flow: Flow);
            }
            catch (Exception exception)
            {
                await LogAsync(
level: WorkflowLogLevel.Error,
message: $"Problem in one or more links for activity {activity.Ref}:{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }
    }

    private static string BuildAssign(Activity activity, Flow flow)
    {
        string[] assignments = activity.Previous?
            .Select(selector: source =>
            {
                Link link = flow.Links.First(predicate: found => found.Source == source.Ref && found.Destination == activity.Ref);
                string sourceType = TypeNameExtensions.GetCSharpTypeName(type: source.GetType());
                string destinationType = TypeNameExtensions.GetCSharpTypeName(type: activity.GetType());

                return string.IsNullOrWhiteSpace(value: link.Expression)
                    ? null
                    : $"//LINK:: {source.Ref} => {activity.Ref}{Environment.NewLine}"
                        + link.Expression
                            .Replace(oldValue: "destination.", newValue: $"(({destinationType})activity).", comparisonType: StringComparison.Ordinal)
                            .Replace(oldValue: "source.", newValue: $"flow.GetActivity<{sourceType}>(\"{source.Ref}\").", comparisonType: StringComparison.Ordinal);
            })
            .Where(predicate: item => item is not null)
            .ToArray()
            ?? [];

        if (assignments.Length == 0)
        {
            return null;
        }

        string body = $"\t{string.Join(separator: $";{Environment.NewLine}\t", value: assignments)}";
        return $"(activity, variables, flow) => {{{Environment.NewLine}{body}{Environment.NewLine}}}";
    }
}