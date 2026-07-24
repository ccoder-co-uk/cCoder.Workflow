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
    public async Task Post_CreatesCalendarEvent()
    {
        // Given
        SeededCalendarEventContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "CalendarEvent");
        CalendarEvent expectedCalendarEvent;
        CalendarEvent actualCalendarEvent;

        // When
        expectedCalendarEvent = await CreateCalendarEventAsync(payload: new
        {
            calendarId = seededContext.CalendarId,
            name,
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(hours: 1).Ticks,
        });

        actualCalendarEvent = await GetCalendarEventAsync(id: expectedCalendarEvent.Id);

        // Then
        actualCalendarEvent.Name.Should().Be(expected: name);

        await DeleteCalendarEventAsync(id: expectedCalendarEvent.Id);
        await Teardown(seededContext: seededContext);
    }
}