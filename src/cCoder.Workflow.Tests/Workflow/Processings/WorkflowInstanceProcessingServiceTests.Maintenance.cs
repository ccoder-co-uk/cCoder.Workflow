// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public sealed partial class WorkflowInstanceProcessingServiceTests
{
    [Fact]
    public void ShouldReturnWorkflowExecutionStatistics()
    {
        // Given
        object[] expected = [new { Failed = 2 }];

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetFailedExecutionStats())
            .Returns(value: expected);

        // When
        object[] actual = processingService.GetStats();

        // Then
        actual.Should().BeSameAs(expected: expected);
        workflowInstanceManagementBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogDroppedWorkflowInstancesDuringMaintenanceAsync()
    {
        // Given
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.FlushOldInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 3);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogInformation(
                "Dropped {Count} Workflow instances older than {MaxAge}.",
                It.IsAny<object[]>()));

        // When
        await processingService.RunInstanceMaintenanceAsync();

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogMaintenanceFailureAndInnerFailureAsync()
    {
        // Given
        Exception innerException = new(message: "inner");
        Exception exception = new(message: "outer", innerException: innerException);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.FlushOldInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Throws(exception: exception);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: exception,
                message: exception.Message));

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: innerException,
                message: innerException.Message));

        // When
        await processingService.RunInstanceMaintenanceAsync();

        // Then
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldSkipContinuousMaintenanceDuringMigrationAsync()
    {
        // Given
        configuration.IsMigrating = true;

        // When
        await processingService.RunInstanceMaintenanceContinuouslyAsync();

        // Then
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSkipContinuousQueueProcessingDuringMigrationAsync()
    {
        // Given
        configuration.IsMigrating = true;

        // When
        await processingService
            .RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync();

        // Then
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldMapInvalidQueuePollingIntervalAsync()
    {
        // Given
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: []);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.RequeueHungExecutingInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(),
                cancellationToken: cancellation.Token))
            .ReturnsAsync(value: 0);

        // When
        Func<Task> action = async () => await processingService
            .RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(
                cancellationToken: cancellation.Token);

        // Then
        await action.Should().ThrowAsync<Exception>();
        workflowInstanceManagementBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldStopContinuousQueueProcessingWhenCancelledAsync()
    {
        // Given
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        configuration.QueueInstanceManagement.PollingIntervalMilliseconds = 10;

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: []);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.RequeueHungExecutingInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(),
                cancellationToken: cancellation.Token))
            .ReturnsAsync(value: 0);

        // When
        await processingService
            .RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(
                cancellationToken: cancellation.Token);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogQueueProcessingFailureAndInnerFailureAsync()
    {
        // Given
        Exception innerException = new(message: "inner");
        Exception exception = new(message: "outer", innerException: innerException);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Throws(exception: exception);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: exception,
                message: exception.Message));

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: innerException,
                message: innerException.Message));

        // When
        await processingService
            .RunQueueInstanceBackgroundServiceDependencyAsync();

        // Then
        loggingBrokerMock.VerifyAll();
    }
}