using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class CalendarControllerTests
{
    [Fact]
    public async Task Patch_UpdatesCalendar()
    {
        // Given
        SeededCalendarContext seededContext = await SeedDatabase();
        Calendar createdCalendar = await CreateCalendarAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Calendar"),
            description = "Acceptance calendar",
        });
        string updatedName = Unique("PatchedCalendar");
        Calendar actualCalendar;

        // When
        await PatchCalendarAsync(createdCalendar.Id, new
        {
            name = updatedName,
        });

        actualCalendar = await GetCalendarAsync(createdCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Name.Should().Be(updatedName);

        await DeleteCalendarAsync(createdCalendar.Id);
        await Teardown(seededContext);
    }
}





