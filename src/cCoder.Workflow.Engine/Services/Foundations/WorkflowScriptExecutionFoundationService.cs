// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using Microsoft.Extensions.Logging;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class WorkflowScriptExecutionFoundationService(
    IScriptBroker scriptBroker,
    IJsonBroker jsonBroker,
    cCoder.Workflow.Engine.Brokers.Loggings.ILoggingBroker logger)
    : IWorkflowScriptExecutionFoundationService
{
    public ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [payload, useDetails]);

            if (useDetails)
            {
                ExecutionDetails details =
                    jsonBroker.DeserializeWithoutTypeInformation<ExecutionDetails>(
                        value: payload)
                    ?? throw new InvalidOperationException(
                        "Workflow script execution details could not be deserialized.");

                return await scriptBroker.Run<string>(
                    code: details.Script,
                    imports: Activity.ScriptImports.ToArray(),
                    args: details.Model,
                    log: Log);
            }

            object result = await scriptBroker.Run<object>(
                code: payload,
                imports: Activity.ScriptImports.ToArray(),
                log: Log);

            return jsonBroker.SerializeForOData(value: result);
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