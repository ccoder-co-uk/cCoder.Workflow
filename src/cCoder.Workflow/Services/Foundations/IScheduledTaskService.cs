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

    bool ExecuteAsUserBelongsToApp(string executeAs, int appId);

    bool FlowBelongsToApp(Guid flowId, int appId);

    ValueTask<ScheduledTask> AddAsync(ScheduledTask scheduledTask);

    ValueTask<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask);

    ValueTask DeleteAsync(int scheduledTaskId);

    ValueTask DeleteAllForAppAsync(IEnumerable<ScheduledTask> items);

    ValueTask DeleteAllByAppIdAsync(int appId);
}