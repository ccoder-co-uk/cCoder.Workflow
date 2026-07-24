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
        CalendarEvent createdCalendarEvent = await CreateCalendarEventAsync(new
        {
            calendarId = seededContext.CalendarId,
            name = Unique("CalendarEvent"),
            description = "Acceptance calendar event",
            start = DateTimeOffset.UtcNow,
            durationInTicks = TimeSpan.FromHours(1).Ticks,
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteCalendarEventAsync(createdCalendarEvent.Id);
        actualReadStatusCode = await GetCalendarEventStatusCodeAsync(createdCalendarEvent.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}