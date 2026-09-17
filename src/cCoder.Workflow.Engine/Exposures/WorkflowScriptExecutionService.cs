// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Services.Foundations;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class WorkflowScriptExecutionService(
    IWorkflowScriptExecutionFoundationService
        workflowScriptExecutionProcessingService)
    : IWorkflowScriptExecutionService
{
    public Task<string> ExecuteAsync(string payload, bool useDetails) =>
        workflowScriptExecutionProcessingService
            .ExecuteWorkflowScriptAsync(
                payload: payload,
                useDetails: useDetails)
            .AsTask();
}