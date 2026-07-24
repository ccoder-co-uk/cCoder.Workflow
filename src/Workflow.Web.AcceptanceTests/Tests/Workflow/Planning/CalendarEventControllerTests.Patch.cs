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
        CalendarEvent createdCalendarEvent = await CreateCalendarEventAsync(new
        {
            calendarId = seededContext.CalendarId,
            name = Unique("CalendarEvent"),
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(1).Ticks,
        });
        string updatedName = Unique("PatchedCalendarEvent");
        CalendarEvent actualCalendarEvent;

        // When
        await PatchCalendarEventAsync(createdCalendarEvent.Id, new
        {
            name = updatedName,
            description = "Patched calendar event",
        });

        actualCalendarEvent = await GetCalendarEventAsync(createdCalendarEvent.Id);

        // Then
        actualCalendarEvent.Name.Should().Be(updatedName);

        await DeleteCalendarEventAsync(createdCalendarEvent.Id);
        await Teardown(seededContext);
    }
}