// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Support;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public sealed class WorkflowScriptExecutionOrchestrationService(
    ILogger<WorkflowScriptExecutionOrchestrationService> logger)
    : IWorkflowScriptExecutionOrchestrationService
{
    private static readonly string[] Imports = Activity.ScriptImports;

    public async Task<string> ExecuteAsync(string payload, bool useDetails)
    {
        ScriptRunner runner = new(LogAsync);

        if (useDetails)
        {
            ExecutionDetails details = JsonConvert.DeserializeObject<ExecutionDetails>(
value: payload,
settings: new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None })
                ?? throw new InvalidOperationException("Workflow script execution details could not be deserialized.");

            return await runner.Run<string>(code: details.Script, imports: Imports, args: details.Model, log: LogSync);
        }

        object result = await runner.Run<object>(code: payload, imports: Imports, log: LogSync);
        return JsonConvert.SerializeObject(value: result, settings: WorkflowJson.GetODataJsonSettings());
    }

    private Task LogAsync(WorkflowLogLevel level, string message)
    {
        LogSync(level: level, message: message);
        return Task.CompletedTask;
    }

    private void LogSync(WorkflowLogLevel level, string message)
    {
        if (level == WorkflowLogLevel.Error || level == WorkflowLogLevel.Fatal)
        {
            logger.LogError(message: "{Message}", args: message);
        }
        else if (level == WorkflowLogLevel.Warning)
        {
            logger.LogWarning(message: "{Message}", args: message);
        }
        else if (level == WorkflowLogLevel.Info)
        {
            logger.LogInformation(message: "{Message}", args: message);
        }
        else
        {
            logger.LogDebug(message: "{Message}", args: message);
        }
    }

    public sealed class ExecutionDetails
    {
        public string Script { get; set; }

        public JObject Model { get; set; }
    }
}