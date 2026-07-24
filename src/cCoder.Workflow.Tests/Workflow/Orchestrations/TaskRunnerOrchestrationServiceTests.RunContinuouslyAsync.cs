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
    public async Task RunContinuouslyAsyncShouldCompleteWhenCancellationIsRequested()
    {
        // Given
        using CancellationTokenSource cancellationTokenSource = new();

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.IsScheduledTaskMigrationActive())
            .Returns(value: false);

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