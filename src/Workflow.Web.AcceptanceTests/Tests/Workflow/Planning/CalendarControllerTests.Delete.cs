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
        Calendar createdCalendar = await CreateCalendarAsync(payload:new
        {
            appId = seededContext.AppId,
            name = Unique("Calendar"),
            description = "Acceptance calendar",
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteCalendarAsync(id:createdCalendar.Id);
        actualReadStatusCode = await GetCalendarStatusCodeAsync(id:createdCalendar.Id);

        // Then
        actualStatusCode.Should().Be(expected:200);
        actualReadStatusCode.Should().Be(expected:404);

        await Teardown(seededContext:seededContext);
    }
}