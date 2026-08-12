// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public async Task ShouldDeleteScheduledTaskAsync()
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectAllScheduledTasksIgnoringQueryFilters())
            .Returns(value: new[] { scheduledTask }
                .AsQueryable());

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: scheduledTask.AppId,
                privilege: "ScheduledTask_delete"));

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.DeleteScheduledTaskAsync(
                deletedEntity: It.Is<ScheduledTask>(match: deleted =>
                    deleted.Id == scheduledTask.Id)))
            .Returns(value: ValueTask.FromResult(result: 1));

        // When
        await scheduledTaskService.DeleteAsync(
            scheduledTaskId: scheduledTask.Id);

        // Then
        scheduledTaskBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreMissingScheduledTaskWhenDeleteAsync()
    {
        // Given
        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectAllScheduledTasksIgnoringQueryFilters())
            .Returns(value: Array.Empty<ScheduledTask>()
                .AsQueryable());

        // When
        await scheduledTaskService.DeleteAsync(scheduledTaskId: 1);

        // Then
        scheduledTaskBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}