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
    public async Task ShouldDelegateScheduledTaskOperationsAsync()
    {
        // Given
        ScheduledTask task = CreateScheduledTask();
        IQueryable<ScheduledTask> tasks = new[] { task }
            .AsQueryable();

        scheduledTaskServiceMock
            .Setup(expression: service => service.Get(
                scheduledTaskId: task.Id))
            .Returns(value: task);

        scheduledTaskServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: tasks);

        scheduledTaskServiceMock
            .Setup(expression: service => service.MarkExecutedAsync(
                scheduledTaskId: task.Id,
                incrementNextExecution: false))
            .Returns(value: ValueTask.FromResult(result: task));

        scheduledTaskServiceMock
            .Setup(expression: service => service.AddScheduledTaskAsync(
                newScheduledTask: task))
            .Returns(value: ValueTask.FromResult(result: task));

        scheduledTaskServiceMock
            .Setup(expression: service => service.UpdateScheduledTaskAsync(
                updatedScheduledTask: task))
            .Returns(value: ValueTask.FromResult(result: task));

        scheduledTaskServiceMock
            .Setup(expression: service => service.DeleteAsync(
                scheduledTaskId: task.Id))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskServiceMock
            .Setup(expression: service => service.DeleteAllByAppIdAsync(
                appId: task.AppId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        ScheduledTask actualGet = processingService.Get(scheduledTaskId: task.Id);

        IQueryable<ScheduledTask> actualAll = processingService.GetAll(
            ignoreFilters: true);

        ScheduledTask actualExecuted = await processingService
            .ExecuteScheduledTaskAsync(
                scheduledTaskId: task.Id,
                incrementNextExecution: false);

        ScheduledTask actualAdded = await processingService.AddScheduledTaskAsync(
            newEntity: task);

        ScheduledTask actualUpdated = await processingService.UpdateScheduledTaskAsync(
            updatedEntity: task);

        await processingService.DeleteAsync(scheduledTaskId: task.Id);
        await processingService.DeleteByAppIdAsync(appId: task.AppId);

        // Then
        actualGet
            .Should()
            .BeSameAs(expected: task);

        actualAll
            .Should()
            .BeSameAs(expected: tasks);

        actualExecuted
            .Should()
            .BeSameAs(expected: task);

        actualAdded
            .Should()
            .BeSameAs(expected: task);

        actualUpdated
            .Should()
            .BeSameAs(expected: task);

        scheduledTaskServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteAllScheduledTasksAsync()
    {
        // Given
        ScheduledTask first = CreateScheduledTask();
        ScheduledTask second = CreateScheduledTask();

        scheduledTaskServiceMock
            .Setup(expression: service => service.DeleteAsync(
                scheduledTaskId: first.Id))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskServiceMock
            .Setup(expression: service => service.DeleteAsync(
                scheduledTaskId: second.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await processingService.DeleteAllScheduledTaskAsync(
            deletedItems: new[] { first, second });

        // Then
        scheduledTaskServiceMock.VerifyAll();
    }
}