// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class ScheduledTaskService(
    IScheduledTaskBroker scheduledTaskBroker,
    IAuthorizationBroker authorizationBroker
) : IScheduledTaskService
{
    public ScheduledTask Get(int scheduledTaskId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [scheduledTaskId]); return ExecuteGet(scheduledTaskId: scheduledTaskId); });

    private ScheduledTask ExecuteGet(int scheduledTaskId)
    {
        ScheduledTask scheduledTask = GetAll()
            .FirstOrDefault(predicate: i => i.Id == scheduledTaskId);

        if (scheduledTask is not null)
        {
            return scheduledTask;
        }

        ScheduledTask unrestrictedScheduledTask = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == scheduledTaskId);

        if (unrestrictedScheduledTask is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public ScheduledTask GetForExecution(int scheduledTaskId) =>
        TryCatch(operation: () => { ValidateForExecutionOnGet(inputs: [scheduledTaskId]); return ExecuteGetForExecution(scheduledTaskId: scheduledTaskId); });

    private ScheduledTask ExecuteGetForExecution(int scheduledTaskId) =>
        scheduledTaskBroker.SelectScheduledTaskForExecution(scheduledTaskId: scheduledTaskId);

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<ScheduledTask> ExecuteGetAll(bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return scheduledTaskBroker
                .SelectAllScheduledTasksIgnoringQueryFilters();
        }

        return scheduledTaskBroker.SelectAllScheduledTasks();
    }

    public ValueTask<ScheduledTask> MarkExecutedAsync(int scheduledTaskId, bool incrementNextExecution) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId, incrementNextExecution]); return await ExecuteMarkExecutedAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteMarkExecutedAsync(
        int scheduledTaskId,
        bool incrementNextExecution)
    {
        ScheduledTask scheduledTask =
            scheduledTaskBroker.SelectScheduledTaskForExecution(
                scheduledTaskId: scheduledTaskId);

        if (scheduledTask is null)
        {
            return null;
        }

        scheduledTask.LastExecuted = DateTimeOffset.UtcNow;

        if (incrementNextExecution)
        {
            while (scheduledTask.NextExecution < DateTimeOffset.UtcNow &&
                scheduledTask.NextExecution is not null)
            {
                scheduledTask.NextExecution = scheduledTask.ScheduleInTicks > 0
                    ? scheduledTask.NextExecution +
                        TimeSpan.FromTicks(value: scheduledTask.ScheduleInTicks)
                    : null;
            }
        }

        return await scheduledTaskBroker.UpdateScheduledTaskAsync(
            entity: scheduledTask);
    }

    public bool GetExecuteAsUserBelongsToApp(string executeAs, int appId) =>
        TryCatch(operation: () => { ValidateExecuteAsUserBelongsToAppOnGet(inputs: [executeAs, appId]); return ExecuteGetExecuteAsUserBelongsToApp(executeAs: executeAs, appId: appId); });

    private bool ExecuteGetExecuteAsUserBelongsToApp(string executeAs, int appId) =>
        scheduledTaskBroker.SelectExecuteAsUserBelongsToApp(executeAs: executeAs, appId: appId);

    public bool GetFlowBelongsToApp(Guid flowId, int appId) =>
        TryCatch(operation: () => { ValidateFlowBelongsToAppOnGet(inputs: [flowId, appId]); return ExecuteGetFlowBelongsToApp(flowId: flowId, appId: appId); });

    private bool ExecuteGetFlowBelongsToApp(Guid flowId, int appId) =>
        scheduledTaskBroker.SelectFlowBelongsToApp(flowId: flowId, appId: appId);

    public ValueTask<ScheduledTask> AddAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); return await ExecuteAddAsync(scheduledTask: scheduledTask); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteAddAsync(ScheduledTask scheduledTask)
    {
        authorizationBroker.Authorize(appId: scheduledTask.AppId, privilege: $"{nameof(ScheduledTask)}_create");
        ScheduledTask newScheduledTask = CreateStorageScheduledTask(item: scheduledTask);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newScheduledTask.Created = now;
        newScheduledTask.CreatedBy = currentUserId;
        newScheduledTask.LastUpdated = now;
        newScheduledTask.UpdatedBy = currentUserId;

        ScheduledTask result = await scheduledTaskBroker.InsertScheduledTaskAsync(entity: newScheduledTask);
        scheduledTask.Id = result.Id;
        scheduledTask.AppId = result.AppId;
        scheduledTask.FlowId = result.FlowId;
        scheduledTask.ExcludedEventsCalendarId = result.ExcludedEventsCalendarId;
        scheduledTask.ExcludedEventsName = result.ExcludedEventsName;
        scheduledTask.Name = result.Name;
        scheduledTask.Description = result.Description;
        scheduledTask.ExecutionArgs = result.ExecutionArgs;
        scheduledTask.ScheduleInTicks = result.ScheduleInTicks;
        scheduledTask.CreatedBy = result.CreatedBy;
        scheduledTask.UpdatedBy = result.UpdatedBy;
        scheduledTask.ExecuteAs = result.ExecuteAs;
        scheduledTask.Created = result.Created;
        scheduledTask.LastUpdated = result.LastUpdated;
        scheduledTask.LastExecuted = result.LastExecuted;
        scheduledTask.NextExecution = result.NextExecution;
        return scheduledTask;
    }

    public ValueTask<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); return await ExecuteUpdateAsync(scheduledTask: scheduledTask); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteUpdateAsync(ScheduledTask scheduledTask)
    {
        authorizationBroker.Authorize(appId: scheduledTask.AppId, privilege: $"{nameof(ScheduledTask)}_update");
        ScheduledTask updateScheduledTask = CreateStorageScheduledTask(item: scheduledTask);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateScheduledTask.LastUpdated = now;
        updateScheduledTask.UpdatedBy = currentUserId;

        ScheduledTask result = await scheduledTaskBroker.UpdateScheduledTaskAsync(
entity: updateScheduledTask
        );

        scheduledTask.Id = result.Id;
        scheduledTask.AppId = result.AppId;
        scheduledTask.FlowId = result.FlowId;
        scheduledTask.ExcludedEventsCalendarId = result.ExcludedEventsCalendarId;
        scheduledTask.ExcludedEventsName = result.ExcludedEventsName;
        scheduledTask.Name = result.Name;
        scheduledTask.Description = result.Description;
        scheduledTask.ExecutionArgs = result.ExecutionArgs;
        scheduledTask.ScheduleInTicks = result.ScheduleInTicks;
        scheduledTask.CreatedBy = result.CreatedBy;
        scheduledTask.UpdatedBy = result.UpdatedBy;
        scheduledTask.ExecuteAs = result.ExecuteAs;
        scheduledTask.Created = result.Created;
        scheduledTask.LastUpdated = result.LastUpdated;
        scheduledTask.LastExecuted = result.LastExecuted;
        scheduledTask.NextExecution = result.NextExecution;
        return scheduledTask;
    }

    public ValueTask DeleteAsync(int scheduledTaskId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId]); await ExecuteDeleteAsync(scheduledTaskId: scheduledTaskId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int scheduledTaskId)
    {
        ScheduledTask scheduledTask = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == scheduledTaskId);

        if (scheduledTask is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: scheduledTask.AppId, privilege: $"{nameof(ScheduledTask)}_delete");

        _ = await scheduledTaskBroker.DeleteScheduledTaskAsync(
entity: CreateStorageScheduledTask(item: scheduledTask)
        );
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateAllForAppOnDelete(inputs: [items]); await ExecuteDeleteAllForAppAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<ScheduledTask> items)
    {
        IEnumerable<ScheduledTask> storageScheduledTasks =
            items?.Select(selector: CreateStorageScheduledTask) ?? [];

        if (!storageScheduledTasks.Any())
        {
            return ValueTask.CompletedTask;
        }

        return scheduledTaskBroker.DeleteAllScheduledTasksAsync(
            items: storageScheduledTasks);
    }

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateAllByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteAllByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllByAppIdAsync(int appId) =>
        scheduledTaskBroker.DeleteAllScheduledTasksByAppIdAsync(appId: appId);

    private static ScheduledTask CreateStorageScheduledTask(ScheduledTask item) =>
        item == null
            ? null
            : new()
            {
                Id = item.Id,
                AppId = item.AppId,
                FlowId = item.FlowId,
                ExcludedEventsCalendarId = item.ExcludedEventsCalendarId,
                ExcludedEventsName = item.ExcludedEventsName,
                Name = item.Name,
                Description = item.Description,
                ExecutionArgs = item.ExecutionArgs,
                ScheduleInTicks = item.ScheduleInTicks,
                CreatedBy = item.CreatedBy,
                UpdatedBy = item.UpdatedBy,
                ExecuteAs = item.ExecuteAs,
                Created = item.Created,
                LastUpdated = item.LastUpdated,
                LastExecuted = item.LastExecuted,
                NextExecution = item.NextExecution,
                ExecuteAsUser = item.ExecuteAsUser,
                App = item.App,
                Flow = item.Flow,
                ExcludedEventsCalendar = item.ExcludedEventsCalendar,
            };
}