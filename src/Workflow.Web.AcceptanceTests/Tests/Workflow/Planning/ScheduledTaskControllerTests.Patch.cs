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
    public async Task Patch_UpdatesScheduledTask()
    {
        // Given
        SeededScheduledTaskContext seededContext = await SeedDatabase();
        ScheduledTask createdScheduledTask = await CreateScheduledTaskAsync(payload:new
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
        string updatedName = Unique(prefix:"PatchedScheduledTask");
        ScheduledTask actualScheduledTask;

        // When
        await PatchScheduledTaskAsync(id:createdScheduledTask.Id, payload:new
        {
            name = updatedName,
            executionArgs = "{\"patched\":true}",
        });

        actualScheduledTask = await GetScheduledTaskAsync(id:createdScheduledTask.Id);

        // Then
        actualScheduledTask.Name.Should().Be(expected:updatedName);

        await DeleteScheduledTaskAsync(id:createdScheduledTask.Id);
        await Teardown(seededContext:seededContext);
    }
}