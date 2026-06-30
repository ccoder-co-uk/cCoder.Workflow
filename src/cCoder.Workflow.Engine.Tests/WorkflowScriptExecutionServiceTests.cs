using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionServiceTests
{
    private readonly Mock<IWorkflowScriptExecutionOrchestrationService> orchestrationServiceMock = new();
    private readonly WorkflowScriptExecutionService workflowScriptExecutionService;

    public WorkflowScriptExecutionServiceTests() =>
        workflowScriptExecutionService = new WorkflowScriptExecutionService(orchestrationServiceMock.Object);
}
