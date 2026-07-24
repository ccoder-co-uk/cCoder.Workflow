// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class ScheduledTaskControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetScheduledTaskCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfScheduledTasks()
    {
        // Given

        // When
        IReadOnlyList<ScheduledTask> actualScheduledTasks = await GetScheduledTasksAsync(1);

        // Then
        actualScheduledTasks.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsScheduledTaskById()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        string name = Unique("ScheduledTask");
        ScheduledTask expectedScheduledTask = await CreateScheduledTaskAsync(new
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
        ScheduledTask actualScheduledTask;

        // When
        actualScheduledTask = await GetScheduledTaskAsync(expectedScheduledTask.Id);

        // Then
        actualScheduledTask.Id.Should().Be(expectedScheduledTask.Id);
        actualScheduledTask.Name.Should().Be(name);

        await DeleteScheduledTaskAsync(expectedScheduledTask.Id);
        await Teardown(seededContext);
    }
}