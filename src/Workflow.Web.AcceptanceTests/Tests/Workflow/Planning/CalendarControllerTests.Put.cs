// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class CalendarControllerTests
{
    [Fact]
    public async Task Put_UpdatesCalendar()
    {
        // Given
        SeededCalendarContext seededContext = await SeedDatabase();
        Calendar createdCalendar = await CreateCalendarAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Calendar"),
            description = "Acceptance calendar",
        });
        string updatedName = Unique("UpdatedCalendar");
        Calendar actualCalendar;

        // When
        await UpdateCalendarAsync(createdCalendar.Id, new
        {
            id = createdCalendar.Id,
            appId = seededContext.AppId,
            name = updatedName,
            description = "Updated calendar",
        });

        actualCalendar = await GetCalendarAsync(createdCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Name.Should().Be(updatedName);

        await DeleteCalendarAsync(createdCalendar.Id);
        await Teardown(seededContext);
    }
}