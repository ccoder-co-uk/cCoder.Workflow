// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Workflow.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

#pragma warning disable STXFORMAT008
public partial class CalendarEventServiceTests
{
    private readonly Mock<ICalendarEventBroker> calendarEventBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly CalendarEventService calendarEventService;

    public CalendarEventServiceTests()
    {

        calendarEventService = new CalendarEventService(
            calendarEventBroker: calendarEventBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);

    }

    private static CalendarEvent CreateCalendarEvent() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            CalendarId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            Name = "Calendar event",
            Description = "Description",
            Start = DateTimeOffset.UtcNow,
            DurationInTicks = TimeSpan.FromMinutes(value: 30).Ticks
        };
}
#pragma warning restore STXFORMAT008