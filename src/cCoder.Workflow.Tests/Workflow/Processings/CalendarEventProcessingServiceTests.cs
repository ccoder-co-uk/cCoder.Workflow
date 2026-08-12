// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT008
public partial class CalendarEventProcessingServiceTests
{
    private readonly Mock<ICalendarEventService> calendarEventServiceMock = new();

    private readonly CalendarEventProcessingService processingService;

    public CalendarEventProcessingServiceTests()
    {
        processingService = new CalendarEventProcessingService(
            service: calendarEventServiceMock.Object);
    }

    private static CalendarEvent CreateCalendarEvent() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            CalendarId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            Name = "Event",
            Description = "Description",
            Start = DateTimeOffset.UtcNow,
            DurationInTicks = TimeSpan.FromMinutes(value: 30).Ticks
        };
}
#pragma warning restore STXFORMAT008