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
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCalendarEventCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCalendarEvents()
    {
        // Given

        // When
        IReadOnlyList<CalendarEvent> actualCalendarEvents = await GetCalendarEventsAsync(top: 1);

        // Then
        actualCalendarEvents.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCalendarEventById()
    {
        // Given
        SeededCalendarEventContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "CalendarEvent");
        CalendarEvent expectedCalendarEvent = await CreateCalendarEventAsync(payload: new
        {
            calendarId = seededContext.CalendarId,
            name,
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(hours: 1).Ticks,
        });
        CalendarEvent actualCalendarEvent;

        // When
        actualCalendarEvent = await GetCalendarEventAsync(calendarEventId: expectedCalendarEvent.Id);

        // Then
        actualCalendarEvent.Id.Should().Be(expected: expectedCalendarEvent.Id);
        actualCalendarEvent.Name.Should().Be(expected: name);

        await DeleteCalendarEventAsync(calendarEventId: expectedCalendarEvent.Id);
        await Teardown(seededContext: seededContext);
    }
}