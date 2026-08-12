// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class ScheduledTaskEventServiceTests
{
    private readonly Mock<IScheduledTaskEventBroker> scheduledTaskEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.ScheduledTaskEventService service;
    private const string CurrentUserId = "test-user";

    public ScheduledTaskEventServiceTests()
    {
        scheduledTaskEventBrokerMock = new Mock<IScheduledTaskEventBroker>(behavior: MockBehavior.Strict);

        scheduledTaskEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.ScheduledTaskEventService(
            scheduledTaskEventBroker: scheduledTaskEventBrokerMock.Object);
    }
}