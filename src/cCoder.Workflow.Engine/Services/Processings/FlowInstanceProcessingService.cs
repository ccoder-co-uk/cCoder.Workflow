// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Reflection;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Support;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowInstanceProcessingService(
    IScriptBroker scriptBroker,
    IWorkflowContextBroker workflowContextBroker)
    : IFlowInstanceProcessingService
{
    public ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            WorkflowRequest request = flowExecution.Request;
            flowExecution.Start = DateTimeOffset.UtcNow;
            flowExecution.Script = scriptBroker;

            using HttpClient api = CreateHttpClient(apiRoot: request.Api);

            api.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: request.AuthToken);

            string rawInstance = await api.GetStringAsync(
                requestUri:
                    $"Workflow/FlowInstanceData({request.InstanceId})"
                    + "?$expand=FlowDefinition($expand=App)");

            FlowInstanceData instanceData =
                await DeserializeFlowInstanceDataAsync(
                    flowExecution: flowExecution,
                    rawInstance: rawInstance);

            PopulateFlowExecution(
                flowExecution: flowExecution,
                instanceData: instanceData);

            WorkflowContext dataContext =
                await DeserializeWorkflowContextAsync(
                    flowExecution: flowExecution,
                    rawContext: instanceData.ContextString);

            flowExecution.Flow = dataContext.Flow
                ?? throw new InvalidOperationException(
                    "Flow instance context did not contain a workflow.");

            await StitchFlowExecutionAsync(
                flowExecution: flowExecution);

            flowExecution.Context =
                workflowContextBroker.CreateWorkflowExecutionContext(
                    flowExecution: flowExecution);

            await workflowContextBroker
                .ExecuteWorkflowExecutionContextAsync(
                    workflowExecutionContext: flowExecution.Context,
                    apiRoot: request.Api,
                    authToken: request.AuthToken);

            flowExecution.Result = CompleteFlowExecution(
                flowExecution: flowExecution);

            return flowExecution;
        });

    private static void PopulateFlowExecution(
        FlowExecution flowExecution,
        FlowInstanceData instanceData)
    {
        flowExecution.AppId = instanceData.FlowDefinition.AppId;
        flowExecution.Id = instanceData.Id;
        flowExecution.Name = instanceData.Name;
        flowExecution.Caller = instanceData.Caller;

        flowExecution.FlowDefinitionId =
            instanceData.FlowDefinitionId;
    }

    private static async Task<FlowInstanceData>
        DeserializeFlowInstanceDataAsync(
            FlowExecution flowExecution,
            string rawInstance)
    {
        try
        {
            return JsonConvert.DeserializeObject<FlowInstanceData>(
                value: rawInstance,
                settings: WorkflowJson.GetJsonSettings())
                ?? throw new InvalidOperationException(
                    "Workflow instance response was empty.");
        }
        catch
        {
            await LogFlowExecutionAsync(
                flowExecution: flowExecution,
                level: WorkflowLogLevel.Error,
                message:
                    $"Failed to deserialize flow instance:"
                    + $"{Environment.NewLine}{rawInstance}");

            throw;
        }
    }

    private static async Task<WorkflowContext>
        DeserializeWorkflowContextAsync(
            FlowExecution flowExecution,
            string rawContext)
    {
        try
        {
            return JsonConvert.DeserializeObject<WorkflowContext>(
                value: rawContext,
                settings: WorkflowJson.GetJsonSettings())
                ?? throw new InvalidOperationException(
                    "Workflow context response was empty.");
        }
        catch
        {
            await LogFlowExecutionAsync(
                flowExecution: flowExecution,
                level: WorkflowLogLevel.Error,
                message:
                    $"Failed to deserialize flow context:"
                    + $"{Environment.NewLine}{rawContext}");

            throw;
        }
    }

    private static FlowInstanceData CompleteFlowExecution(
        FlowExecution flowExecution)
    {
        foreach (Activity activity in
            flowExecution.Context.Flow.Activities)
        {
            PropertyInfo[] properties = activity.GetType()
                .GetProperties()
                .Where(predicate: property =>
                    property.GetCustomAttribute<
                        IgnoreWhenFlowCompleteAttribute>() is not null)
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                property.SetValue(
                    obj: activity,
                    value: default);
            }
        }

        return new FlowInstanceData
        {
            Id = flowExecution.Id,
            Name = flowExecution.Name,
            Caller = flowExecution.Caller,
            FlowDefinitionId = flowExecution.FlowDefinitionId,
            ContextString = JsonConvert.SerializeObject(
                value: flowExecution.Context,
                settings: WorkflowJson.GetJsonSettings()),
            State = flowExecution.Context.ExecutionState,
            Start = flowExecution.Start,
            End = DateTimeOffset.UtcNow
        };
    }

    private static async Task StitchFlowExecutionAsync(
        FlowExecution flowExecution)
    {
        foreach (Activity activity in flowExecution.Flow.Activities)
        {
            try
            {
                string[] links = flowExecution.Flow.Links
                    .Where(predicate: link =>
                        link.Destination == activity.Ref)
                    .Select(selector: link => link.Source)
                    .ToArray();

                activity.Previous = flowExecution.Flow.Activities
                    .Where(predicate: candidate =>
                        links.Contains(value: candidate.Ref))
                    .ToArray();
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in previous activity selection for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }
        }

        foreach (Activity activity in flowExecution.Flow.Activities)
        {
            try
            {
                activity.Next = flowExecution.Flow.Activities
                    .Where(predicate: candidate =>
                        candidate.Previous?.Contains(
                            value: activity) ?? false)
                    .ToArray();
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in next activity selection for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }

            try
            {
                activity.AssignCode = BuildActivityAssignment(
                    activity: activity,
                    flow: flowExecution.Flow);
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in one or more links for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }
        }
    }

    private static string BuildActivityAssignment(
        Activity activity,
        Flow flow)
    {
        string[] assignments = activity.Previous?
            .Select(selector: source =>
            {
                Link link = flow.Links.First(
                    predicate: found =>
                        found.Source == source.Ref
                        && found.Destination == activity.Ref);

                string sourceType =
                    TypeNameExtensions.GetCSharpTypeName(
                        type: source.GetType());

                string destinationType =
                    TypeNameExtensions.GetCSharpTypeName(
                        type: activity.GetType());

                return string.IsNullOrWhiteSpace(
                    value: link.Expression)
                    ? null
                    : $"//LINK:: {source.Ref} => {activity.Ref}"
                        + Environment.NewLine
                        + link.Expression
                            .Replace(
                                oldValue: "destination.",
                                newValue:
                                    $"(({destinationType})activity).",
                                comparisonType:
                                    StringComparison.Ordinal)
                            .Replace(
                                oldValue: "source.",
                                newValue:
                                    $"flow.GetActivity<{sourceType}>"
                                    + $"(\"{source.Ref}\").",
                                comparisonType:
                                    StringComparison.Ordinal);
            })
            .Where(predicate: item => item is not null)
            .ToArray()
            ?? [];

        if (assignments.Length == 0)
        {
            return null;
        }

        string body = $"\t{string.Join(
            separator: $";{Environment.NewLine}\t",
            value: assignments)}";

        return $"(activity, variables, flow) => "
            + $"{{{Environment.NewLine}{body}"
            + $"{Environment.NewLine}}}";
    }

    private static Task LogFlowExecutionAsync(
        FlowExecution flowExecution,
        WorkflowLogLevel level,
        string message) =>
        flowExecution.Log(
            level: level,
            message: message);

    private static HttpClient CreateHttpClient(
        string apiRoot) =>
        new(new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip
                | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback =
                CertChainValidator.ValidateCertChain
        })
        {
            BaseAddress = new Uri(apiRoot)
        };
}