using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class ScheduledTaskControllerTests
{
    [Fact]
    public async Task Delete_RemovesScheduledTask()
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
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteScheduledTaskAsync(createdScheduledTask.Id);
        actualReadStatusCode = await GetScheduledTaskStatusCodeAsync(createdScheduledTask.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





