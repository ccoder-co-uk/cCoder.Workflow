// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using System.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public async Task MarkExecutedAsyncIgnoresRequestAuthorizationAsExpected()
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();
        DateTimeOffset originalLastExecuted = scheduledTask.LastExecuted.Value;

        scheduledTaskBrokerMock
            .Setup(expression: broker =>
                broker.SelectScheduledTaskForExecution(
                    scheduledTaskId: scheduledTask.Id))
            .Returns(value: scheduledTask);

        scheduledTaskBrokerMock
            .Setup(expression: broker =>
                broker.UpdateScheduledTaskAsync(
                    updatedEntity: scheduledTask))
            .Returns(value: ValueTask.FromResult(result: scheduledTask));

        // When
        ScheduledTask actualScheduledTask =
            await scheduledTaskService.MarkExecutedAsync(
                scheduledTaskId: scheduledTask.Id,
                incrementNextExecution: true);

        // Then
        actualScheduledTask.Should()
            .BeSameAs(expected: scheduledTask);

        actualScheduledTask.LastExecuted.Should()
            .BeAfter(expected: originalLastExecuted);

        authorizationBrokerMock.VerifyNoOtherCalls();
        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotIncrementNextExecutionWhenMarkExecutedAsync()
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();
        DateTimeOffset? expectedNextExecution = scheduledTask.NextExecution;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectScheduledTaskForExecution(
                    scheduledTaskId: scheduledTask.Id))
            .Returns(value: scheduledTask);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.UpdateScheduledTaskAsync(
                updatedEntity: scheduledTask))
            .Returns(value: ValueTask.FromResult(result: scheduledTask));

        // When
        ScheduledTask actual = await scheduledTaskService.MarkExecutedAsync(
            scheduledTaskId: scheduledTask.Id,
            incrementNextExecution: false);

        // Then
        actual.NextExecution
            .Should()
            .Be(expected: expectedNextExecution);

        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldClearNextExecutionForNonRepeatingScheduledTaskAsync()
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();
        scheduledTask.ScheduleInTicks = 0;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectScheduledTaskForExecution(
                    scheduledTaskId: scheduledTask.Id))
            .Returns(value: scheduledTask);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.UpdateScheduledTaskAsync(
                updatedEntity: scheduledTask))
            .Returns(value: ValueTask.FromResult(result: scheduledTask));

        // When
        ScheduledTask actual = await scheduledTaskService.MarkExecutedAsync(
            scheduledTaskId: scheduledTask.Id,
            incrementNextExecution: true);

        // Then
        actual.NextExecution
            .Should()
            .BeNull();

        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRejectMissingScheduledTaskWhenMarkExecutedAsync()
    {
        // Given
        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectScheduledTaskForExecution(scheduledTaskId: 1))
            .Returns(value: null);

        // When
        Func<Task> action = async () => await scheduledTaskService
            .MarkExecutedAsync(
                scheduledTaskId: 1,
                incrementNextExecution: true);

        // Then
        await action
            .Should()
            .ThrowAsync<SecurityException>();

        scheduledTaskBrokerMock.VerifyAll();
    }
}