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
    public async Task Delete_RemovesScheduledTask()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        ScheduledTask createdScheduledTask = await CreateScheduledTaskAsync(payload: new
        {
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name = Unique(prefix: "ScheduledTask"),
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
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteScheduledTaskAsync(scheduledTaskId: createdScheduledTask.Id);
        actualReadStatusCode = await GetScheduledTaskStatusCodeAsync(scheduledTaskId: createdScheduledTask.Id);

        // Then
        actualStatusCode.Should()
            .Be(expected: 200);
        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}