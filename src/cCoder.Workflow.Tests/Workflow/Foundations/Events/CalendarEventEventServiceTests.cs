// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class CalendarEventEventServiceTests
{
    private readonly Mock<ICalendarEventEventBroker> calendarEventEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.CalendarEventEventService service;
    private const string CurrentUserId = "test-user";

    public CalendarEventEventServiceTests()
    {
        calendarEventEventBrokerMock = new Mock<ICalendarEventEventBroker>(behavior: MockBehavior.Strict);

        calendarEventEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.CalendarEventEventService(
            calendarEventEventBroker: calendarEventEventBrokerMock.Object);
    }
}