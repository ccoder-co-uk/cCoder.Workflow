// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Processings;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class WorkflowScriptExecutionOrchestrationAdapter(
    IWorkflowScriptExecutionProcessingService
        workflowScriptExecutionProcessingService)
    : IWorkflowScriptExecutionOrchestrationService
{
    public Task<string> ExecuteAsync(
        string payload,
        bool useDetails) =>
        workflowScriptExecutionProcessingService
            .ExecuteWorkflowScriptAsync(
                payload: payload,
                useDetails: useDetails)
            .AsTask();
}