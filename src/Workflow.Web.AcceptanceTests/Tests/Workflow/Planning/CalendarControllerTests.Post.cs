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
        string name = Unique("Calendar");
        Calendar expectedCalendar;
        Calendar actualCalendar;

        // When
        expectedCalendar = await CreateCalendarAsync(new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance calendar",
        });

        actualCalendar = await GetCalendarAsync(expectedCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Name.Should().Be(name);

        await DeleteCalendarAsync(expectedCalendar.Id);
        await Teardown(seededContext);
    }
}





