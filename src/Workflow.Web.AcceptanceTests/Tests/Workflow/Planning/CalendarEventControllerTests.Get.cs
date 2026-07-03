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
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCalendarEvents()
    {
        // Given

        // When
        IReadOnlyList<CalendarEvent> actualCalendarEvents = await GetCalendarEventsAsync(1);

        // Then
        actualCalendarEvents.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCalendarEventById()
    {
        // Given
        SeededCalendarEventContext seededContext = await SeedDatabase();
        string name = Unique("CalendarEvent");
        CalendarEvent expectedCalendarEvent = await CreateCalendarEventAsync(new
        {
            calendarId = seededContext.CalendarId,
            name,
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(1).Ticks,
        });
        CalendarEvent actualCalendarEvent;

        // When
        actualCalendarEvent = await GetCalendarEventAsync(expectedCalendarEvent.Id);

        // Then
        actualCalendarEvent.Id.Should().Be(expectedCalendarEvent.Id);
        actualCalendarEvent.Name.Should().Be(name);

        await DeleteCalendarEventAsync(expectedCalendarEvent.Id);
        await Teardown(seededContext);
    }
}





