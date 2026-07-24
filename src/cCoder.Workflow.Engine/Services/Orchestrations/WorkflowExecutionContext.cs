// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Services.Processings;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public sealed class WorkflowExecutionContext : WorkflowContext, IWorkflowContext
{
    public WorkflowExecutionContext()
    {
        ExecutionLog = new List<WorkflowLogEntry>();
        Variables = new Dictionary<string, object>
        {
            ["Imports"] = Activity.ScriptImports
        };
    }

    public WorkflowExecutionContext(Flow flow, FlowInstance instance)
        : this()
    {
        Flow = flow;
        InstanceId = instance.Id;
        Instance = instance;
    }

    [JsonIgnore]
    public IScriptRunner Script => Instance?.Script ?? new ScriptRunner((level, message) =>
    {
        Log(level:level, message:message);
        return Task.CompletedTask;
    });

    [JsonIgnore]
    internal FlowInstance Instance { get; private set; }

    public async Task ExecuteAsync(string apiRoot, string authToken = null)
    {
        try
        {
            Log(level:WorkflowLogLevel.Info, message:"Execution started");
            Variables["AppId"] = Instance.AppId;
            Variables["Api"] = apiRoot;
            Variables["AuthToken"] = authToken;
            Variables["InstanceId"] = InstanceId.ToString();

            using HttpClient api = Instance.CreateApiClient(apiRoot:apiRoot);
            api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            User user = await api.GetAsync<User>(query:"AppSecurity/User/Me()");
            Instance.Caller = user.Id;
            Variables["UserId"] = user.Id;
            Variables["UserName"] = user.DisplayName;
            Variables["UserEmail"] = user.Email;

            Start start = Flow.Activities.OfType<Start>().First();

            if (authToken is not null)
                start.AuthToken = authToken;

            if (Variables.TryGetValue(key:"Data", value:out object data))
                start.Data = data;

            await start.ExecuteInternal(context:this);
        }
        catch (Exception exception)
        {
            Log(level:WorkflowLogLevel.Error, message:"Execution failed.");
            Log(level:WorkflowLogLevel.Error, message:$"{exception.Message}{Environment.NewLine}{exception.StackTrace}");

            Exception inner = exception.InnerException;
            while (inner is not null)
            {
                Log(level:WorkflowLogLevel.Error, message:$"{inner.Message}{Environment.NewLine}{inner.StackTrace}");
                inner = inner.InnerException;
            }
        }

        EvaluateFinalState();
    }

    public void Log(WorkflowLogLevel level, string message)
    {
        ExecutionLog.Add(item:new WorkflowLogEntry(level, message));
        Instance?.LogAsync(level:level, message:message).GetAwaiter().GetResult();
    }

    private void EvaluateFinalState()
    {
        if (Flow.Activities.All(predicate:activity => activity.State is ActivityState.Complete or ActivityState.Skipped))
        {
            Log(level:WorkflowLogLevel.Info, message:"Execution complete.");
            ExecutionState = ExecutionLog.Any(predicate:entry => entry.Level == "Warn")
                ? "CompletedWithWarnings"
                : "Complete";
            return;
        }

        ExecutionState = "Failed";
    }
}