// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;

namespace Workflow.Brokers.WorkflowScriptExecutions;

internal sealed class WorkflowScriptExecutionBroker(
    IWorkflowScriptExecutionService workflowScriptExecutionService)
        : IWorkflowScriptExecutionBroker
{
    public Task<string> ExecuteScriptAsync(string payload, bool useDetails) =>
        workflowScriptExecutionService.ExecuteAsync(
            payload: payload,
            useDetails: useDetails);
}