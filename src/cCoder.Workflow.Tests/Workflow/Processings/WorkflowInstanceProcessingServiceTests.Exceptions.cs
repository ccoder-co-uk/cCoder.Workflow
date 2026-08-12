// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public sealed partial class WorkflowInstanceProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowInstanceDataManagerMock
            .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        // When
        Action action = () => processingService.GetAll();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapExecuteWaitingQueuedInstanceFailureAsync(
        Exception exception,
        Type expectedType)
    {
        // Given
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.UpdateQueuedInstanceClaimAsync(
                flowInstanceDataId: It.IsAny<Guid>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService
            .ExecuteWaitingQueuedInstanceByIdAsync(
                flowInstanceDataId: Guid.NewGuid());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown.Should().BeOfType(expectedType: expectedType);
    }

    [Fact]
    public async Task ShouldStopContinuousMaintenanceWhenCancelledAsync()
    {
        // Given
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.FlushOldInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(),
                cancellationToken: cancellation.Token))
            .ReturnsAsync(value: 0);

        // When
        await processingService.RunInstanceMaintenanceContinuouslyAsync(
            cancellationToken: cancellation.Token);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
    }
}