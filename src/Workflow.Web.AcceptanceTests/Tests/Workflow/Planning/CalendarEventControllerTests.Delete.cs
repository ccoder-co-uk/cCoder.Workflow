// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class CalendarEventControllerTests
{
    [Fact]
    public async Task Delete_RemovesCalendarEvent()
    {
        // Given
        SeededCalendarEventContext seededContext = await SeedDatabase();
        CalendarEvent createdCalendarEvent = await CreateCalendarEventAsync(payload: new
        {
            calendarId = seededContext.CalendarId,
            name = Unique(prefix: "CalendarEvent"),
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(hours: 1).Ticks,
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteCalendarEventAsync(id: createdCalendarEvent.Id);
        actualReadStatusCode = await GetCalendarEventStatusCodeAsync(id: createdCalendarEvent.Id);

        // Then
        actualStatusCode.Should().Be(expected: 200);
        actualReadStatusCode.Should().Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}