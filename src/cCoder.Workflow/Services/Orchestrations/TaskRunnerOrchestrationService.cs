// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Services.Orchestrations;

internal sealed class TaskRunnerOrchestrationService(
    IScheduledTaskService scheduledTaskService,
    ICalendarEventService calendarEventService,
    IScheduledTaskEventProcessingService scheduledTaskEventProcessingService,
    WorkflowConfiguration configuration,
    ILogger<TaskRunnerOrchestrationService> log)
    : ITaskRunnerOrchestrationService
{
    public async Task RunContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (configuration.IsMigrating)
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

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        ScheduledTask[] dueTasks = scheduledTaskService.GetAll(ignoreFilters: true)
            .Where(predicate: task => task.NextExecution != null && task.NextExecution < DateTimeOffset.UtcNow && task.ScheduleInTicks != 0)
            .Include(navigationPropertyPath: task => task.Flow)
                .ThenInclude(navigationPropertyPath: flow => flow.App)
            .Include(navigationPropertyPath: task => task.ExecuteAsUser)
                .ThenInclude(navigationPropertyPath: user => user.Roles)
                    .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .ToArray();

        if (dueTasks.Length == 0)
        {
            log.LogDebug(message: "No scheduled tasks are due to run.");
            return;
        }

        int[] calendarIds = dueTasks
            .Where(predicate: task => task.ExcludedEventsCalendarId != null)
            .Select(selector: task => task.ExcludedEventsCalendarId.Value)
            .Distinct()
            .ToArray();

        CalendarEvent[] events = calendarEventService.GetAll(ignoreFilters: true)
            .Where(predicate: calendarEvent =>
                calendarIds.Contains(value: calendarEvent.CalendarId) &&
                calendarEvent.Start >= DateTimeOffset.Now.Date &&
                calendarEvent.Start <= DateTimeOffset.Now.AddDays(days: 14).Date)
            .ToArray();

        log.LogInformation(message: "{Count} are scheduled to run, executing ...", args: dueTasks.Length);

        int dueTasksExecuted = 0;

        foreach (ScheduledTask task in dueTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            log.LogDebug(
                "Running task {Name} ({Id}), due to be run since @ {DueTime}",
                task.Name,
                task.Id,
                task.NextExecution?.ToString(format: "HH:mm:ss"));

            await RunTaskAsync(events: events, task: task, cancellationToken: cancellationToken);
            dueTasksExecuted++;

            log.LogDebug("Running task {Name} ({Id}) complete", task.Name, task.Id);
        }

        log.LogInformation(message: "{Count} Scheduled executions run.", args: dueTasksExecuted);
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
                log.LogDebug(
                    "Task {Id} - {Name} in app {AppId} skipped due to excluded date",
                    task.Id,
                    task.Name,
                    task.AppId);

                return;
            }
        }

        await ExecuteTaskAsync(task: task, cancellationToken: cancellationToken);
    }

    private async Task ExecuteTaskAsync(
        ScheduledTask task,
        CancellationToken cancellationToken)
    {
        ScheduledTask updatedTask = await scheduledTaskService.MarkExecutedAsync(
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