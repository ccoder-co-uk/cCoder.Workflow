// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Models;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Dependencies;

public sealed class WorkflowExecutionContext : WorkflowContext, IWorkflowContext
{
    private readonly FlowExecution flowExecution;

    public WorkflowExecutionContext()
    {
        ExecutionLog = new List<WorkflowLogEntry>();
        Variables = new Dictionary<string, object>
        {
            ["Imports"] = Activity.ScriptImports
        };
    }

    public WorkflowExecutionContext(
        FlowExecution flowExecution)
        : this()
    {
        this.flowExecution = flowExecution;
        Flow = flowExecution.Flow;
        InstanceId = flowExecution.Id;
    }

    [JsonIgnore]
    public IScriptRunner Script => flowExecution.Script;

    public async Task ExecuteAsync(string apiRoot, string authToken = null)
    {
        try
        {
            Log(level: WorkflowLogLevel.Info, message: "Execution started");
            Variables["AppId"] = flowExecution.AppId;
            Variables["Api"] = apiRoot;
            Variables["AuthToken"] = authToken;
            Variables["InstanceId"] = InstanceId.ToString();

            using HttpClient api = CreateApiClient(apiRoot: apiRoot);
            api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            User user = await api.GetAsync<User>(query: "AppSecurity/User/Me()");
            flowExecution.Caller = user.Id;
            Variables["UserId"] = user.Id;
            Variables["UserName"] = user.DisplayName;
            Variables["UserEmail"] = user.Email;

            Start start = Flow.Activities.OfType<Start>()
                .First();

            if (authToken is not null)
            {
                start.AuthToken = authToken;
            }

            if (Variables.TryGetValue(key: "Data", value: out object data))
            {
                start.Data = data;
            }

            await start.ExecuteInternal(context: this);
        }
        catch (Exception exception)
        {
            Log(level: WorkflowLogLevel.Error, message: "Execution failed.");
            Log(level: WorkflowLogLevel.Error, message: $"{exception.Message}{Environment.NewLine}{exception.StackTrace}");

            Exception inner = exception.InnerException;

            while (inner is not null)
            {
                Log(level: WorkflowLogLevel.Error, message: $"{inner.Message}{Environment.NewLine}{inner.StackTrace}");
                inner = inner.InnerException;
            }
        }

        EvaluateFinalState();
    }

    public void Log(WorkflowLogLevel level, string message)
    {
        ExecutionLog.Add(item: new WorkflowLogEntry(level, message));

        flowExecution.Log(level: level, message: message)
            .GetAwaiter()
            .GetResult();
    }

    private void EvaluateFinalState()
    {
        if (Flow.Activities.All(predicate: activity => activity.State is ActivityState.Complete or ActivityState.Skipped))
        {
            Log(level: WorkflowLogLevel.Info, message: "Execution complete.");

            ExecutionState = ExecutionLog.Any(predicate: entry => entry.Level == "Warn")
                ? "CompletedWithWarnings"
                : "Complete";

            return;
        }

        ExecutionState = "Failed";
    }

    private static HttpClient CreateApiClient(
        string apiRoot) =>
        new(new HttpClientHandler
        {
            AutomaticDecompression =
                System.Net.DecompressionMethods.GZip
                | System.Net.DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback =
                CertChainValidator.ValidateCertChain
        })
        {
            BaseAddress = new Uri(apiRoot)
        };
}