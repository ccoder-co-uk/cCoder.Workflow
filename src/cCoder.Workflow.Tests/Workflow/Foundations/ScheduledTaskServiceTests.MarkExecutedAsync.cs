// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
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
}