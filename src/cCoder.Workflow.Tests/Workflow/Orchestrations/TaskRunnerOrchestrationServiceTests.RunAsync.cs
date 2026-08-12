// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Services.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT009
public partial class TaskRunnerOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldExecuteDueScheduledTaskAsync()
    {
        // Given
        ScheduledTask task = CreateDueScheduledTask();

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { task }.AsQueryable());

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<CalendarEvent>().AsQueryable());

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTasksRunningAsync(
                scheduledTaskCount: 1))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskRunningAsync(
                scheduledTask: task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.ExecuteScheduledTaskAsync(
                scheduledTaskId: task.Id,
                incrementNextExecution: true))
            .Returns(value: ValueTask.FromResult(result: task));

        scheduledTaskEventProcessingServiceMock
            .Setup(expression: service => service
                .RaiseScheduledTaskExecuteEventAsync(entity: task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskCompleteAsync(
                scheduledTask: task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTasksExecutedAsync(
                scheduledTaskCount: 1))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await taskRunnerOrchestrationService.RunAsync();

        // Then
        scheduledTaskProcessingServiceMock.VerifyAll();
        scheduledTaskEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldSkipScheduledTaskForMatchingCalendarEventAsync()
    {
        // Given
        ScheduledTask task = CreateDueScheduledTask();
        task.ExcludedEventsCalendarId = 3;
        task.ExcludedEventsName = "Holiday,Shutdown";

        CalendarEvent calendarEvent = new()
        {
            CalendarId = 3,
            Name = "Holiday",
            Start = DateTimeOffset.Now.Date
        };

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { task }.AsQueryable());

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { calendarEvent }.AsQueryable());

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTasksRunningAsync(1))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskRunningAsync(task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskSkippedAsync(task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskCompleteAsync(task))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTasksExecutedAsync(1))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await taskRunnerOrchestrationService.RunAsync();

        // Then
        scheduledTaskProcessingServiceMock.VerifyAll();
        scheduledTaskEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRejectMissingUpdatedScheduledTaskAsync()
    {
        // Given
        ScheduledTask task = CreateDueScheduledTask();
        SetupDueTask(task: task);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.ExecuteScheduledTaskAsync(
                scheduledTaskId: task.Id,
                incrementNextExecution: true))
            .Returns(value: ValueTask.FromResult<ScheduledTask>(result: null));

        // When
        Func<Task> action = async () => await taskRunnerOrchestrationService.RunAsync();

        // Then
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ShouldRejectScheduledTaskWithMissingUserAsync()
    {
        // Given
        ScheduledTask task = CreateDueScheduledTask();
        task.ExecuteAsUser = null;
        SetupDueTask(task: task);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.ExecuteScheduledTaskAsync(
                scheduledTaskId: task.Id,
                incrementNextExecution: true))
            .Returns(value: ValueTask.FromResult(result: task));

        // When
        Func<Task> action = async () => await taskRunnerOrchestrationService.RunAsync();

        // Then
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ShouldSkipContinuousRunnerDuringMigrationAsync()
    {
        // Given
        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.IsScheduledTaskMigrationActive())
            .Returns(value: true);

        // When
        await taskRunnerOrchestrationService.RunContinuouslyAsync();

        // Then
        scheduledTaskProcessingServiceMock.VerifyAll();
    }

    private void SetupDueTask(ScheduledTask task)
    {
        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { task }.AsQueryable());

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<CalendarEvent>().AsQueryable());

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTasksRunningAsync(1))
            .Returns(value: ValueTask.CompletedTask);

        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.LogScheduledTaskRunningAsync(task))
            .Returns(value: ValueTask.CompletedTask);
    }

    private static ScheduledTask CreateDueScheduledTask() =>
        new()
        {
            Id = 1,
            NextExecution = DateTimeOffset.UtcNow.AddMinutes(minutes: -1),
            ScheduleInTicks = TimeSpan.FromMinutes(value: 5).Ticks,
            ExecuteAsUser = new User(),
            Flow = new FlowDefinition()
        };
}
#pragma warning restore STXFORMAT005, STXFORMAT009