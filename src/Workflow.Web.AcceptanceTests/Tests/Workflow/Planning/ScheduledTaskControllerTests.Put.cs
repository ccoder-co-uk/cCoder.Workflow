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
    public async Task Put_UpdatesScheduledTask()
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
        string updatedName = Unique(prefix: "UpdatedScheduledTask");
        ScheduledTask actualScheduledTask;

        // When
        await UpdateScheduledTaskAsync(id: createdScheduledTask.Id, payload: new
        {
            id = createdScheduledTask.Id,
            appId = seededContext.AppId,
            flowId = seededContext.FlowId,
            name = updatedName,
            description = "Updated scheduled task",
            executionArgs = "{\"updated\":true}",
            scheduleInTicks = TimeSpan.FromHours(hours: 2).Ticks,
            executeAs = "Guest",
            createdBy = "Guest",
            updatedBy = "Guest",
            created = DateTimeOffset.UtcNow,
            lastUpdated = DateTimeOffset.UtcNow,
            nextExecution = DateTimeOffset.UtcNow.AddHours(hours: 2),
        });

        actualScheduledTask = await GetScheduledTaskAsync(id: createdScheduledTask.Id);

        // Then
        actualScheduledTask.Name.Should().Be(expected: updatedName);

        await DeleteScheduledTaskAsync(id: createdScheduledTask.Id);
        await Teardown(seededContext: seededContext);
    }
}