// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class FlowRunner(IWorkflowRequestOrchestrationService workflowRequestOrchestrationService)
    : IFlowRunner
{
    public Task RunAsync(WorkflowRequest workflowRequest) =>
        workflowRequestOrchestrationService
            .ExecuteWorkflowRequestAsync(
                workflowRequest: workflowRequest)
            .AsTask();
}