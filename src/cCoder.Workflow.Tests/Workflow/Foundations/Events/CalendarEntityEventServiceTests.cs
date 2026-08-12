// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class CalendarEntityEventServiceTests
{
    private readonly Mock<ICalendarEntityEventBroker> calendarEntityEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.CalendarEntityEventService service;
    private const string CurrentUserId = "test-user";

    public CalendarEntityEventServiceTests()
    {
        calendarEntityEventBrokerMock = new Mock<ICalendarEntityEventBroker>(behavior: MockBehavior.Strict);

        calendarEntityEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.CalendarEntityEventService(
            calendarEventBroker: calendarEntityEventBrokerMock.Object);
    }
}