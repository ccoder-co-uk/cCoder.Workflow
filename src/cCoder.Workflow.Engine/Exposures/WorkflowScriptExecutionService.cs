// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Exposures;

public sealed class WorkflowScriptExecutionService(
    IWorkflowScriptExecutionOrchestrationService workflowScriptExecutionOrchestrationService)
    : IWorkflowScriptExecutionService
{
    public Task<string> ExecuteAsync(string payload, bool useDetails) =>
        workflowScriptExecutionOrchestrationService.ExecuteAsync(payload: payload, useDetails: useDetails);
}