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
        Calendar createdCalendar = await CreateCalendarAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "Calendar"),
            description = "Acceptance calendar",
        });
        string updatedName = Unique(prefix: "UpdatedCalendar");
        Calendar actualCalendar;

        // When
        await UpdateCalendarAsync(id: createdCalendar.Id, payload: new
        {
            id = createdCalendar.Id,
            appId = seededContext.AppId,
            name = updatedName,
            description = "Updated calendar",
        });

        actualCalendar = await GetCalendarAsync(id: createdCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Name.Should().Be(expected: updatedName);

        await DeleteCalendarAsync(id: createdCalendar.Id);
        await Teardown(seededContext: seededContext);
    }
}