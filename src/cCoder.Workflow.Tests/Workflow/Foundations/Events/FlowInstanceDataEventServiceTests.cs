// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowInstanceDataEventServiceTests
{
    private readonly Mock<IFlowInstanceDataEventBroker> flowInstanceDataEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.FlowInstanceDataEventService service;
    private const string CurrentUserId = "test-user";

    public FlowInstanceDataEventServiceTests()
    {
        flowInstanceDataEventBrokerMock = new Mock<IFlowInstanceDataEventBroker>(MockBehavior.Strict);

        flowInstanceDataEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.FlowInstanceDataEventService(
            flowInstanceDataEventBroker: flowInstanceDataEventBrokerMock.Object);
    }
}