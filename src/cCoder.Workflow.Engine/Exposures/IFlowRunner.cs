using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Exposures;

public interface IFlowRunner
{
    Task RunAsync(WorkflowRequest request);
}
