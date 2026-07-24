// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

public interface IScheduledTaskService
{
    ScheduledTask Get(int scheduledTaskId);

    ScheduledTask GetForExecution(int scheduledTaskId);

    IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false);

    ValueTask<ScheduledTask> MarkExecutedAsync(int scheduledTaskId, bool incrementNextExecution);

    bool GetExecuteAsUserBelongsToApp(string executeAs, int appId);

    bool GetFlowBelongsToApp(Guid flowId, int appId);

    ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newScheduledTask);

    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedScheduledTask);

    ValueTask DeleteAsync(int scheduledTaskId);

    ValueTask DeleteAllForAppScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems);

    ValueTask DeleteAllByAppIdAsync(int appId);
}