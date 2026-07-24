// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class TaskRunnerOrchestrationService(
    IScheduledTaskProcessingService scheduledTaskProcessingService,
    ICalendarEventProcessingService calendarEventProcessingService,
    IScheduledTaskEventProcessingService scheduledTaskEventProcessingService)
    : ITaskRunnerOrchestrationService
{
    public Task RunContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunContinuouslyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (scheduledTaskProcessingService.IsScheduledTaskMigrationActive())
        {
            return;
        }

        await RunAsync(cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await RunAsync(cancellationToken: cancellationToken);
        }
    }

    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunAsync(CancellationToken cancellationToken = default)
    {
        ScheduledTask[] dueTasks = scheduledTaskProcessingService.GetAll(ignoreFilters: true)
            .Where(predicate: task => task.NextExecution != null && task.NextExecution < DateTimeOffset.UtcNow && task.ScheduleInTicks != 0)
            .Include(navigationPropertyPath: task => task.Flow)
                .ThenInclude(navigationPropertyPath: flow => flow.App)
            .Include(navigationPropertyPath: task => task.ExecuteAsUser)
                .ThenInclude(navigationPropertyPath: user => user.Roles)
                    .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .ToArray();

        if (dueTasks.Length == 0)
        {
            await scheduledTaskProcessingService.LogNoScheduledTasksDueAsync();
            return;
        }

        int[] calendarIds = dueTasks
            .Where(predicate: task => task.ExcludedEventsCalendarId != null)
            .Select(selector: task => task.ExcludedEventsCalendarId.Value)
            .Distinct()
            .ToArray();

        CalendarEvent[] events = calendarEventProcessingService.GetAll(ignoreFilters: true)
            .Where(predicate: calendarEvent =>
                calendarIds.Contains(value: calendarEvent.CalendarId) &&
                calendarEvent.Start >= DateTimeOffset.Now.Date &&
                calendarEvent.Start <= DateTimeOffset.Now.AddDays(days: 14).Date)
            .ToArray();

        await scheduledTaskProcessingService.LogScheduledTasksRunningAsync(scheduledTaskCount: dueTasks.Length);

        int dueTasksExecuted = 0;

        foreach (ScheduledTask task in dueTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await scheduledTaskProcessingService.LogScheduledTaskRunningAsync(scheduledTask: task);

            await RunTaskAsync(events: events, task: task, cancellationToken: cancellationToken);
            dueTasksExecuted++;

            await scheduledTaskProcessingService.LogScheduledTaskCompleteAsync(scheduledTask: task);
        }

        await scheduledTaskProcessingService.LogScheduledTasksExecutedAsync(scheduledTaskCount: dueTasksExecuted);
    }

    private async Task RunTaskAsync(
        CalendarEvent[] events,
        ScheduledTask task,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(value: task.ExcludedEventsName))
        {
            string[] eventNames = task.ExcludedEventsName.Split(separator: ",");

            CalendarEvent[] matchedEvents = task.ExcludedEventsCalendarId != null
                ? events
                    .Where(predicate: calendarEvent =>
                        calendarEvent.CalendarId == task.ExcludedEventsCalendarId &&
                        eventNames.Contains(value: calendarEvent.Name))
                    .ToArray()
                : [];

            if (matchedEvents.Any(predicate: calendarEvent => calendarEvent.Start.Date == DateTimeOffset.Now.Date))
            {
                await scheduledTaskProcessingService.LogScheduledTaskSkippedAsync(scheduledTask: task);

                return;
            }
        }

        await ExecuteTaskAsync(task: task, cancellationToken: cancellationToken);
    }

    private async Task ExecuteTaskAsync(
        ScheduledTask task,
        CancellationToken cancellationToken)
    {
        ScheduledTask updatedTask = await scheduledTaskProcessingService.ExecuteScheduledTaskAsync(
            scheduledTaskId: task.Id,
            incrementNextExecution: true);

        if (updatedTask == null)
        {
            throw new InvalidOperationException(
                $"Scheduled task {task.Id} could not be marked as executed.");
        }

        if (task.ExecuteAsUser == null)
        {
            throw new InvalidOperationException("User doesn't exist.");
        }

        await scheduledTaskEventProcessingService.RaiseScheduledTaskExecuteEventAsync(entity: updatedTask);
    }
}