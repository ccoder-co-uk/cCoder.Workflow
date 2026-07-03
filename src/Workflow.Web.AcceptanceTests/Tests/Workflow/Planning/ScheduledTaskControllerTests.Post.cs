using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class ScheduledTaskControllerTests
{
    [Fact]
    public async Task Post_CreatesScheduledTask()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        string name = Unique("ScheduledTask");
        ScheduledTask expectedScheduledTask;
        ScheduledTask actualScheduledTask;

        // When
        expectedScheduledTask = await CreateScheduledTaskAsync(new
        {
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name,
            description = "Acceptance scheduled task",
            executionArgs = "{}",
            scheduleInTicks = TimeSpan.FromHours(1).Ticks,
            executeAs = "Guest",
            createdBy = "Guest",
            updatedBy = "Guest",
            created = DateTimeOffset.UtcNow,
            lastUpdated = DateTimeOffset.UtcNow,
            nextExecution = DateTimeOffset.UtcNow.AddHours(1),
        });

        actualScheduledTask = await GetScheduledTaskAsync(expectedScheduledTask.Id);

        // Then
        actualScheduledTask.Name.Should().Be(name);

        await DeleteScheduledTaskAsync(expectedScheduledTask.Id);
        await Teardown(seededContext);
    }
}





