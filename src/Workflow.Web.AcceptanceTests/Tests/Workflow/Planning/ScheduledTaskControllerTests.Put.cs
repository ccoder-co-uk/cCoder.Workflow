using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class ScheduledTaskControllerTests
{
    [Fact]
    public async Task Put_UpdatesScheduledTask()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        ScheduledTask createdScheduledTask = await CreateScheduledTaskAsync(new
        {
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name = Unique("ScheduledTask"),
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
        string updatedName = Unique("UpdatedScheduledTask");
        ScheduledTask actualScheduledTask;

        // When
        await UpdateScheduledTaskAsync(createdScheduledTask.Id, new
        {
            id = createdScheduledTask.Id,
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name = updatedName,
            description = "Updated scheduled task",
            executionArgs = "{\"updated\":true}",
            scheduleInTicks = TimeSpan.FromHours(2).Ticks,
            executeAs = "Guest",
            createdBy = "Guest",
            updatedBy = "Guest",
            created = DateTimeOffset.UtcNow,
            lastUpdated = DateTimeOffset.UtcNow,
            nextExecution = DateTimeOffset.UtcNow.AddHours(2),
        });

        actualScheduledTask = await GetScheduledTaskAsync(createdScheduledTask.Id);

        // Then
        actualScheduledTask.Name.Should().Be(updatedName);

        await DeleteScheduledTaskAsync(createdScheduledTask.Id);
        await Teardown(seededContext);
    }
}





