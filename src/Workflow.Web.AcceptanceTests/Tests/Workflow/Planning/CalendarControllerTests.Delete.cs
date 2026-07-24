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
    public async Task Delete_RemovesCalendar()
    {
        // Given
        SeededCalendarContext seededContext = await SeedDatabase();
        Calendar createdCalendar = await CreateCalendarAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Calendar"),
            description = "Acceptance calendar",
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteCalendarAsync(createdCalendar.Id);
        actualReadStatusCode = await GetCalendarStatusCodeAsync(createdCalendar.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}