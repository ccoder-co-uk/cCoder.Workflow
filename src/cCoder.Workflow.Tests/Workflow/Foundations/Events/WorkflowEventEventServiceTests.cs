using cCoder.Workflow.Brokers.Events;
using cCoder.Data;
using Moq;
using cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class WorkflowEventEventServiceTests
{
    private readonly Mock<IWorkflowEventEventBroker> workflowEventEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.WorkflowEventEventService service;
    private const string CurrentUserId = "test-user";

    public WorkflowEventEventServiceTests()
    {
        workflowEventEventBrokerMock = new Mock<IWorkflowEventEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        workflowEventEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Workflow.Services.Foundations.Events.WorkflowEventEventService(
            workflowEventEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









