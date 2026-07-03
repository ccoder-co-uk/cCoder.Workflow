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
        CalendarEvent createdCalendarEvent = await CreateCalendarEventAsync(new
        {
            calendarId = seededContext.CalendarId,
            name = Unique("CalendarEvent"),
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(1).Ticks,
        });
        string updatedName = Unique("UpdatedCalendarEvent");
        CalendarEvent actualCalendarEvent;

        // When
        await UpdateCalendarEventAsync(createdCalendarEvent.Id, new
        {
            id = createdCalendarEvent.Id,
            calendarId = seededContext.CalendarId,
            name = updatedName,
            description = "Updated calendar event",
            start = DateTimeOffset.UtcNow.AddHours(1),
            durationInTicks = TimeSpan.FromHours(2).Ticks,
        });

        actualCalendarEvent = await GetCalendarEventAsync(createdCalendarEvent.Id);

        // Then
        actualCalendarEvent.Name.Should().Be(updatedName);

        await DeleteCalendarEventAsync(createdCalendarEvent.Id);
        await Teardown(seededContext);
    }
}





