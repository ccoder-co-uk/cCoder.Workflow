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
    public async Task ShouldDeleteAllForAppScheduledTaskAsync()
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();
        IEnumerable<ScheduledTask> captured = null;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.DeleteAllScheduledTasksAsync(
                deletedItems: It.IsAny<IEnumerable<ScheduledTask>>()))
            .Callback<IEnumerable<ScheduledTask>>(action: deletedItems =>
                captured = deletedItems)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await scheduledTaskService.DeleteAllForAppScheduledTaskAsync(
            deletedItems: new[] { scheduledTask });

        // Then
        captured
            .Should()
            .ContainSingle(predicate: deleted =>
                deleted.Id == scheduledTask.Id);

        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreEmptyDeleteAllForAppScheduledTaskAsync()
    {
        // Given

        // When
        await scheduledTaskService.DeleteAllForAppScheduledTaskAsync(
            deletedItems: Array.Empty<ScheduledTask>());

        // Then
        scheduledTaskBrokerMock.VerifyNoOtherCalls();
    }
}