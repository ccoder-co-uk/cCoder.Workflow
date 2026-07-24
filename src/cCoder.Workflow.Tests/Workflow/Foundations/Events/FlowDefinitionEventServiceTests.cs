// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowDefinitionEventServiceTests
{
    private readonly Mock<IFlowDefinitionEventBroker> flowDefinitionEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.FlowDefinitionEventService service;
    private const string CurrentUserId = "test-user";

    public FlowDefinitionEventServiceTests()
    {
        flowDefinitionEventBrokerMock = new Mock<IFlowDefinitionEventBroker>(behavior: MockBehavior.Strict);

        flowDefinitionEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.FlowDefinitionEventService(
            flowDefinitionEventBroker: flowDefinitionEventBrokerMock.Object);
    }
}