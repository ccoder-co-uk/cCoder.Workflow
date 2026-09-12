// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface IScheduledTaskBroker
{
    IQueryable<ScheduledTask> SelectAllScheduledTasks();

    IQueryable<ScheduledTask> SelectAllScheduledTasksIgnoringQueryFilters();

    ScheduledTask[] SelectDueScheduledTasks(DateTimeOffset currentDateTime);

    ScheduledTask SelectScheduledTaskForExecution(int scheduledTaskId);

    bool SelectExecuteAsUserBelongsToApp(string executeAs, int appId);

    bool SelectFlowBelongsToApp(Guid flowId, int appId);

    ValueTask<ScheduledTask> InsertScheduledTaskAsync(ScheduledTask newScheduledTask);

    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedScheduledTask);

    ValueTask<int> DeleteScheduledTaskAsync(ScheduledTask deletedScheduledTask);

    ValueTask DeleteAllScheduledTasksAsync(IEnumerable<ScheduledTask> deletedItems);

    ValueTask DeleteAllScheduledTasksByAppIdAsync(int appId);

    int? SelectAppId(ScheduledTask scheduledTask);
}