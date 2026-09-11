// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class FlowExecutionOrchestrationAdapter(
    IWorkflowRequestOrchestrationService
        workflowRequestOrchestrationService)
    : IFlowExecutionOrchestrationService
{
    public Task ExecuteAsync(
        WorkflowRequest workflowRequest) =>
        workflowRequestOrchestrationService
            .ExecuteWorkflowRequestAsync(
                workflowRequest: workflowRequest)
            .AsTask();
}