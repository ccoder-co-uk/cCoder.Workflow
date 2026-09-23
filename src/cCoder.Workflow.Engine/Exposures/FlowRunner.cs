// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Coordinations;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class FlowRunner(IWorkflowRequestCoordinationService workflowRequestCoordinationService)
    : IFlowRunner
{
    public Task RunAsync(WorkflowRequest workflowRequest) =>
        workflowRequestCoordinationService
            .ExecuteWorkflowRequestAsync(
                workflowRequest: workflowRequest)
            .AsTask();
}