using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowRunnerTests
{
    private readonly Mock<IFlowExecutionOrchestrationService> flowExecutionOrchestrationServiceMock = new();
    private readonly FlowRunner flowRunner;

    public FlowRunnerTests() =>
        flowRunner = new FlowRunner(flowExecutionOrchestrationServiceMock.Object);

    private static WorkflowRequest CreateWorkflowRequest() =>
        new()
        {
            InstanceId = Guid.NewGuid(),
            Api = "https://localhost/",
            AuthToken = "token",
        };
}
