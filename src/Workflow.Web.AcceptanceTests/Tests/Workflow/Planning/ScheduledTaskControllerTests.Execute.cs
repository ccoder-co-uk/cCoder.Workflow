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
    public async Task Execute_QueuesWorkflowInstance()
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
            nextExecution = DateTimeOffset.UtcNow.AddHours(-2),
        });
        int actualStatusCode;
        ScheduledTask actualScheduledTask;

        // When
        actualStatusCode = await ExecuteScheduledTaskAsync(id:createdScheduledTask.Id, incrementNextExecution: true);

        // Then
        actualScheduledTask = await GetScheduledTaskAsync(id:createdScheduledTask.Id);

        actualStatusCode.Should().Be(expected:200);
        actualScheduledTask.LastExecuted.Should().NotBeNull();
        actualScheduledTask.NextExecution.Should().BeAfter(expected:createdScheduledTask.NextExecution!.Value);

        await DeleteScheduledTaskAsync(id:createdScheduledTask.Id);
        await Teardown(seededContext:seededContext);
    }
}