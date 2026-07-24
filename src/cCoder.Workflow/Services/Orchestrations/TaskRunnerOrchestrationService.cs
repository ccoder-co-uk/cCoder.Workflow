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
            return;

        await RunAsync(cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            await RunAsync(cancellationToken);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        ScheduledTask[] dueTasks = scheduledTaskService.GetAll(true)
            .Where(task => task.NextExecution != null && task.NextExecution < DateTimeOffset.UtcNow && task.ScheduleInTicks != 0)
            .Include(task => task.Flow)
                .ThenInclude(flow => flow.App)
            .Include(task => task.ExecuteAsUser)
                .ThenInclude(user => user.Roles)
                    .ThenInclude(userRole => userRole.Role)
            .ToArray();

        if (dueTasks.Length == 0)
        {
            log.LogDebug("No scheduled tasks are due to run.");
            return;
        }

        int[] calendarIds = dueTasks
            .Where(task => task.ExcludedEventsCalendarId != null)
            .Select(task => task.ExcludedEventsCalendarId.Value)
            .Distinct()
            .ToArray();

        CalendarEvent[] events = calendarEventService.GetAll(true)
            .Where(calendarEvent =>
                calendarIds.Contains(calendarEvent.CalendarId) &&
                calendarEvent.Start >= DateTimeOffset.Now.Date &&
                calendarEvent.Start <= DateTimeOffset.Now.AddDays(14).Date)
            .ToArray();

        log.LogInformation("{Count} are scheduled to run, executing ...", dueTasks.Length);

        int dueTasksExecuted = 0;

        foreach (ScheduledTask task in dueTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            log.LogDebug(
                "Running task {Name} ({Id}), due to be run since @ {DueTime}",
                task.Name,
                task.Id,
                task.NextExecution?.ToString("HH:mm:ss"));

            await RunTaskAsync(events, task, cancellationToken);
            dueTasksExecuted++;

            log.LogDebug("Running task {Name} ({Id}) complete", task.Name, task.Id);
        }

        log.LogInformation("{Count} Scheduled executions run.", dueTasksExecuted);
    }

    private async Task RunTaskAsync(
        CalendarEvent[] events,
        ScheduledTask task,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(task.ExcludedEventsName))
        {
            string[] eventNames = task.ExcludedEventsName.Split(",");
            CalendarEvent[] matchedEvents = task.ExcludedEventsCalendarId != null
                ? events
                    .Where(calendarEvent =>
                        calendarEvent.CalendarId == task.ExcludedEventsCalendarId &&
                        eventNames.Contains(calendarEvent.Name))
                    .ToArray()
                : [];

            if (matchedEvents.Any(calendarEvent => calendarEvent.Start.Date == DateTimeOffset.Now.Date))
            {
                log.LogDebug(
                    "Task {Id} - {Name} in app {AppId} skipped due to excluded date",
                    task.Id,
                    task.Name,
                    task.AppId);
                return;
            }
        }

        await ExecuteTaskAsync(task, cancellationToken);
    }

    private async Task ExecuteTaskAsync(
        ScheduledTask task,
        CancellationToken cancellationToken)
    {
        ScheduledTask updatedTask = await scheduledTaskService.MarkExecutedAsync(
            task.Id,
            incrementNextExecution: true);

        if (updatedTask == null)
            throw new InvalidOperationException(
                $"Scheduled task {task.Id} could not be marked as executed.");

        if (task.ExecuteAsUser == null)
            throw new InvalidOperationException("User doesn't exist.");

        await scheduledTaskEventProcessingService.RaiseScheduledTaskExecuteEventAsync(updatedTask);
    }
}