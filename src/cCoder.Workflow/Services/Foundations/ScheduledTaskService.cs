using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal class ScheduledTaskService(
    IScheduledTaskBroker scheduledTaskBroker,
    IAuthorizationBroker authorizationBroker
) : IScheduledTaskService
{
    public ScheduledTask Get(int id)
    {
        ScheduledTask scheduledTask = GetAll().FirstOrDefault(i => i.Id == id);
        if (scheduledTask is not null)
            return scheduledTask;

        ScheduledTask unrestrictedScheduledTask = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedScheduledTask is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public ScheduledTask GetForExecution(int id) =>
        scheduledTaskBroker.GetScheduledTaskForExecution(id);

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false) =>
        scheduledTaskBroker.GetAllScheduledTasks(ignoreFilters);

    public async ValueTask<ScheduledTask> MarkExecutedAsync(int id, bool incrementNextExecution) =>
        await scheduledTaskBroker.MarkScheduledTaskExecutedAsync(id, incrementNextExecution);

    public bool ExecuteAsUserBelongsToApp(string executeAs, int appId) =>
        scheduledTaskBroker.ExecuteAsUserBelongsToApp(executeAs, appId);

    public bool FlowBelongsToApp(Guid flowId, int appId) =>
        scheduledTaskBroker.FlowBelongsToApp(flowId, appId);

    public async ValueTask<ScheduledTask> AddAsync(ScheduledTask scheduledTask)
    {
        authorizationBroker.Authorize(scheduledTask.AppId, $"{nameof(ScheduledTask)}_create");
        ScheduledTask newScheduledTask = CreateStorageScheduledTask(scheduledTask);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newScheduledTask.Created = now;
        newScheduledTask.CreatedBy = currentUserId;
        newScheduledTask.LastUpdated = now;
        newScheduledTask.UpdatedBy = currentUserId;

        ScheduledTask result = await scheduledTaskBroker.AddScheduledTaskAsync(newScheduledTask);
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

    public async ValueTask<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask)
    {
        authorizationBroker.Authorize(scheduledTask.AppId, $"{nameof(ScheduledTask)}_update");
        ScheduledTask updateScheduledTask = CreateStorageScheduledTask(scheduledTask);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateScheduledTask.LastUpdated = now;
        updateScheduledTask.UpdatedBy = currentUserId;

        ScheduledTask result = await scheduledTaskBroker.UpdateScheduledTaskAsync(
            updateScheduledTask
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

    public async ValueTask DeleteAsync(int id)
    {
        ScheduledTask scheduledTask = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (scheduledTask is null)
            return;

        authorizationBroker.Authorize(scheduledTask.AppId, $"{nameof(ScheduledTask)}_delete");
        _ = await scheduledTaskBroker.DeleteScheduledTaskAsync(
            CreateStorageScheduledTask(scheduledTask)
        );
    }

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
