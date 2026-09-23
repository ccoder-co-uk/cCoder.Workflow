// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.Brokers.WorkflowScriptExecutions;

namespace Workflow.Services.Foundations.WorkflowScriptExecutions;

internal sealed partial class WorkflowScriptExecutionService(
    IWorkflowScriptExecutionBroker workflowScriptExecutionBroker)
        : IWorkflowScriptExecutionService
{
    public Task<string> ExecuteScriptAsync(string payload, bool useDetails) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload, useDetails]);

            return workflowScriptExecutionBroker.ExecuteScriptAsync(
                payload: payload,
                useDetails: useDetails);
        });
}