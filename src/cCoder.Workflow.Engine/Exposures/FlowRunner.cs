// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Coordinations;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class FlowRunner(IWorkflowRequestCoordinationService workflowRequestCoordinationService)
    : IFlowRunner, ICompositionExposure
{
    public Task RunAsync(WorkflowRequest workflowRequest) =>
        workflowRequestCoordinationService
            .ExecuteWorkflowRequestAsync(
                workflowRequest: workflowRequest)
            .AsTask();
}