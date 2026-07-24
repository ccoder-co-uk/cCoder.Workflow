// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Support;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class WorkflowScriptExecutionProcessingService(
    IScriptBroker scriptBroker,
    ILogger<WorkflowScriptExecutionProcessingService> logger)
    : IWorkflowScriptExecutionProcessingService
{
    private static readonly string[] Imports = Activity.ScriptImports;

    public ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [payload, useDetails]);

            if (useDetails)
            {
                ExecutionDetails details =
                    JsonConvert.DeserializeObject<ExecutionDetails>(
                        value: payload,
                        settings: new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.None
                        })
                    ?? throw new InvalidOperationException(
                        "Workflow script execution details could not be deserialized.");

                return await scriptBroker.Run<string>(
                    code: details.Script,
                    imports: Imports,
                    args: details.Model,
                    log: Log);
            }

            object result = await scriptBroker.Run<object>(
                code: payload,
                imports: Imports,
                log: Log);

            return JsonConvert.SerializeObject(
                value: result,
                settings: WorkflowJson.GetODataJsonSettings());
        });

    private void Log(
        WorkflowLogLevel level,
        string message)
    {
        if (level is WorkflowLogLevel.Error or WorkflowLogLevel.Fatal)
        {
            logger.LogError(
                message: "{Message}",
                args: message);
        }
        else if (level == WorkflowLogLevel.Warning)
        {
            logger.LogWarning(
                message: "{Message}",
                args: message);
        }
        else if (level == WorkflowLogLevel.Info)
        {
            logger.LogInformation(
                message: "{Message}",
                args: message);
        }
        else
        {
            logger.LogDebug(
                message: "{Message}",
                args: message);
        }
    }
}