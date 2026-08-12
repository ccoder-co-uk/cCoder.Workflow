// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005
public partial class ScheduledTaskOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateScheduledTaskOperationsAsync()
    {
        // Given
        ScheduledTask task = CreateScheduledTask();
        IQueryable<ScheduledTask> tasks = new[] { task }.AsQueryable();
        Result<ScheduledTask>[] results = [new() { Success = true, Item = task }];

        processingServiceMock.Setup(expression: found => found.Get(task.Id)).Returns(task);
        processingServiceMock.Setup(expression: found => found.GetAll(true)).Returns(tasks);
        processingServiceMock.Setup(expression: found => found.AddScheduledTaskAsync(task))
            .ReturnsAsync(task);
        processingServiceMock.Setup(expression: found => found.UpdateScheduledTaskAsync(task))
            .ReturnsAsync(task);
        processingServiceMock.Setup(expression: found => found.DeleteByAppIdAsync(7))
            .Returns(value: ValueTask.CompletedTask);
        processingServiceMock.Setup(expression: found => found.AddOrUpdateScheduledTask(tasks))
            .ReturnsAsync(results);
        processingServiceMock.Setup(expression: found => found.DeleteAllScheduledTaskAsync(tasks))
            .Returns(value: ValueTask.CompletedTask);
        processingServiceMock.Setup(expression: found => found.ExecuteScheduledTaskAsync(task.Id, false))
            .ReturnsAsync(task);

        eventServiceMock.Setup(expression: found => found.RaiseScheduledTaskAddEventAsync(task))
            .Returns(value: ValueTask.CompletedTask);
        eventServiceMock.Setup(expression: found => found.RaiseScheduledTaskUpdateEventAsync(task))
            .Returns(value: ValueTask.CompletedTask);
        eventServiceMock.Setup(expression: found => found.RaiseScheduledTaskExecuteEventAsync(task))
            .Returns(value: ValueTask.CompletedTask);

        // When
        ScheduledTask actualGet = service.Get(scheduledTaskId: task.Id);
        IQueryable<ScheduledTask> actualAll = service.GetAll(ignoreFilters: true);
        ScheduledTask actualAdd = await service.AddScheduledTaskAsync(newEntity: task);
        ScheduledTask actualUpdate = await service.UpdateScheduledTaskAsync(updatedEntity: task);
        await service.DeleteByAppIdAsync(appId: 7);
        IEnumerable<Result<ScheduledTask>> actualResults = await service
            .AddOrUpdateScheduledTask(items: tasks);
        await service.DeleteAllScheduledTaskAsync(deletedItems: tasks);
        await service.ExecuteAsync(scheduledTaskId: task.Id, incrementNextExecution: false);

        // Then
        actualGet.Should().BeSameAs(expected: task);
        actualAll.Should().BeSameAs(expected: tasks);
        actualAdd.Should().BeSameAs(expected: task);
        actualUpdate.Should().BeSameAs(expected: task);
        actualResults.Should().BeSameAs(expected: results);
        processingServiceMock.VerifyAll();
        eventServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005