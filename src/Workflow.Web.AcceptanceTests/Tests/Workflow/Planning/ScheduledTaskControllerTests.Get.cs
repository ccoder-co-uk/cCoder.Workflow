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
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfScheduledTasks()
    {
        // Given

        // When
        IReadOnlyList<ScheduledTask> actualScheduledTasks = await GetScheduledTasksAsync(top: 1);

        // Then
        actualScheduledTasks.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsScheduledTaskById()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "ScheduledTask");

        ScheduledTask expectedScheduledTask = await CreateScheduledTaskAsync(payload: new
        {
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name,
            description = "Acceptance scheduled task",
            executionArgs = "{}",
            scheduleInTicks = TimeSpan.FromHours(hours: 1).Ticks,
            executeAs = "Guest",
            createdBy = "Guest",
            updatedBy = "Guest",
            created = DateTimeOffset.UtcNow,
            lastUpdated = DateTimeOffset.UtcNow,
            nextExecution = DateTimeOffset.UtcNow.AddHours(hours: 1),
        });

        ScheduledTask actualScheduledTask;

        // When
        actualScheduledTask = await GetScheduledTaskAsync(scheduledTaskId: expectedScheduledTask.Id);

        // Then
        actualScheduledTask.Id.Should()
            .Be(expected: expectedScheduledTask.Id);

        actualScheduledTask.Name.Should()
            .Be(expected: name);

        await DeleteScheduledTaskAsync(scheduledTaskId: expectedScheduledTask.Id);
        await Teardown(seededContext: seededContext);
    }
}