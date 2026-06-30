using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Exposures;

public sealed class FlowRunner(IFlowExecutionOrchestrationService flowExecutionOrchestrationService)
    : IFlowRunner
{
    public Task RunAsync(WorkflowRequest request) =>
        flowExecutionOrchestrationService.ExecuteAsync(request);
}
