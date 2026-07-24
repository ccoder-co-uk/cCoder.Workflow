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
    public async Task Patch_UpdatesCalendarEvent()
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
        string updatedName = Unique(prefix: "PatchedCalendarEvent");
        CalendarEvent actualCalendarEvent;

        // When
        await PatchCalendarEventAsync(id: createdCalendarEvent.Id, payload: new
        {
            name = updatedName,
            description = "Patched calendar event",
        });

        actualCalendarEvent = await GetCalendarEventAsync(id: createdCalendarEvent.Id);

        // Then
        actualCalendarEvent.Name.Should().Be(expected: updatedName);

        await DeleteCalendarEventAsync(id: createdCalendarEvent.Id);
        await Teardown(seededContext: seededContext);
    }
}