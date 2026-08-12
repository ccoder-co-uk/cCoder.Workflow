// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class ScheduledTaskProcessingServiceTests
{
    [Fact]
    public void ShouldGetScheduledTaskMigrationState()
    {
        // Given
        configuration.IsMigrating = true;
        // When
        bool actual = processingService.IsScheduledTaskMigrationActive();

        // Then
        actual
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task ShouldLogScheduledTaskLifecycleAsync()
    {
        // Given
        ScheduledTask task = CreateScheduledTask();
        task.NextExecution = DateTimeOffset.UtcNow;
        // When
        await processingService.LogNoScheduledTasksDueAsync();
        await processingService.LogScheduledTasksRunningAsync(scheduledTaskCount: 1);
        await processingService.LogScheduledTaskRunningAsync(scheduledTask: task);
        await processingService.LogScheduledTaskCompleteAsync(scheduledTask: task);
        await processingService.LogScheduledTaskSkippedAsync(scheduledTask: task);
        await processingService.LogScheduledTasksExecutedAsync(scheduledTaskCount: 1);

        // Then
        loggingBrokerMock.Verify(
            expression: broker => broker.LogDebug(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Exactly(callCount: 4));

        loggingBrokerMock.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Exactly(callCount: 2));
    }
}