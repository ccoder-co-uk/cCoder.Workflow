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
    public async Task Put_UpdatesCalendarEvent()
    {
        // Given
        SeededCalendarEventContext seededContext = await SeedDatabase();
        CalendarEvent createdCalendarEvent = await CreateCalendarEventAsync(payload:new
        {
            calendarId = seededContext.CalendarId,
            name = Unique("CalendarEvent"),
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(1).Ticks,
        });
        string updatedName = Unique(prefix:"UpdatedCalendarEvent");
        CalendarEvent actualCalendarEvent;

        // When
        await UpdateCalendarEventAsync(id:createdCalendarEvent.Id, payload:new
        {
            id = createdCalendarEvent.Id,
            calendarId = seededContext.CalendarId,
            name = updatedName,
            description = "Updated calendar event",
            start = DateTimeOffset.UtcNow.AddHours(1),
            durationInTicks = TimeSpan.FromHours(2).Ticks,
        });

        actualCalendarEvent = await GetCalendarEventAsync(id:createdCalendarEvent.Id);

        // Then
        actualCalendarEvent.Name.Should().Be(expected:updatedName);

        await DeleteCalendarEventAsync(id:createdCalendarEvent.Id);
        await Teardown(seededContext:seededContext);
    }
}