// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface IScheduledTaskBroker
{
    IQueryable<ScheduledTask> SelectAllScheduledTasks(bool ignoreFilters);

    ScheduledTask SelectScheduledTaskForExecution(int scheduledTaskId);

    ValueTask<ScheduledTask> UpdateScheduledTaskExecutionAsync(int scheduledTaskId, bool incrementNextExecution);

    bool SelectExecuteAsUserBelongsToApp(string executeAs, int appId);

    bool SelectFlowBelongsToApp(Guid flowId, int appId);

    ValueTask<ScheduledTask> InsertScheduledTaskAsync(ScheduledTask entity);

    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask entity);

    ValueTask<int> DeleteScheduledTaskAsync(ScheduledTask entity);

    ValueTask DeleteAllScheduledTasksAsync(IEnumerable<ScheduledTask> items);

    ValueTask DeleteAllScheduledTasksByAppIdAsync(int appId);

    int? SelectAppId(ScheduledTask entity);
}