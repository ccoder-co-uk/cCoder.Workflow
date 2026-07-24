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
    public async Task Post_CreatesCalendar()
    {
        // Given
        SeededCalendarContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "Calendar");
        Calendar expectedCalendar;
        Calendar actualCalendar;

        // When
        expectedCalendar = await CreateCalendarAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance calendar",
        });

        actualCalendar = await GetCalendarAsync(id: expectedCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Name.Should().Be(expected: name);

        await DeleteCalendarAsync(id: expectedCalendar.Id);
        await Teardown(seededContext: seededContext);
    }
}