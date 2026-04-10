using cCoder.Workflow.Brokers.Events;
using cCoder.Data;
using Moq;
using cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowInstanceDataEventServiceTests
{
    private readonly Mock<IFlowInstanceDataEventBroker> flowInstanceDataEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.FlowInstanceDataEventService service;
    private const string CurrentUserId = "test-user";

    public FlowInstanceDataEventServiceTests()
    {
        flowInstanceDataEventBrokerMock = new Mock<IFlowInstanceDataEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        flowInstanceDataEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Workflow.Services.Foundations.Events.FlowInstanceDataEventService(
            flowInstanceDataEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









