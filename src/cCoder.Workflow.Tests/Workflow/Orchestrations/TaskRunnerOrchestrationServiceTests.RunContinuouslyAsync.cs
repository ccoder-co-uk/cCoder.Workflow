// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Services.Orchestrations;

public partial class TaskRunnerOrchestrationServiceTests
{
    [Fact]
    public async Task RunContinuouslyAsyncShouldUseConfiguredPollingInterval()
    {
        // Given
        using CancellationTokenSource cancellationTokenSource = new();
        int runCount = 0;

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.IsScheduledTaskMigrationActive())
            .Returns(value: false);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetScheduledTaskPollingInterval())
            .Returns(value: TimeSpan.FromMilliseconds(milliseconds: 10));

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(
                value: Array.Empty<ScheduledTask>()
                    .AsQueryable());

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogNoScheduledTasksDueAsync())
            .Returns(
                valueFunction: () =>
                {
                    runCount++;

                    if (runCount == 2)
                    {
                        cancellationTokenSource.Cancel();
                    }

                    return ValueTask.CompletedTask;
                });

        // When
        Func<Task> runContinuouslyAsync = () =>
            taskRunnerOrchestrationService.RunContinuouslyAsync(
                cancellationToken: cancellationTokenSource.Token);

        // Then
        await runContinuouslyAsync.Should()
            .NotThrowAsync();

        runCount.Should()
            .Be(expected: 2);
    }

    [Fact]
    public async Task RunContinuouslyAsyncShouldCompleteWhenCancellationIsRequested()
    {
        // Given
        using CancellationTokenSource cancellationTokenSource = new();

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.IsScheduledTaskMigrationActive())
            .Returns(value: false);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetScheduledTaskPollingInterval())
            .Returns(value: TimeSpan.FromMinutes(minutes: 1));

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(
                value: Array.Empty<ScheduledTask>()
                    .AsQueryable());

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogNoScheduledTasksDueAsync())
            .Returns(value: ValueTask.CompletedTask);

        cancellationTokenSource.Cancel();

        // When
        Func<Task> runContinuouslyAsync = () =>
            taskRunnerOrchestrationService.RunContinuouslyAsync(
                cancellationToken: cancellationTokenSource.Token);

        // Then
        await runContinuouslyAsync.Should()
            .NotThrowAsync();
    }
}