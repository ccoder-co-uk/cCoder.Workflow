// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

internal interface IScheduledTaskProcessingService
{
    object CreateSingleResult<T>(IQueryable<T> queryable);

    bool IsScheduledTaskMigrationActive();

    TimeSpan GetScheduledTaskPollingInterval();

    ValueTask LogNoScheduledTasksDueAsync();

    ValueTask LogScheduledTasksRunningAsync(int scheduledTaskCount);

    ValueTask LogScheduledTaskRunningAsync(ScheduledTask scheduledTask);

    ValueTask LogScheduledTaskCompleteAsync(ScheduledTask scheduledTask);

    ValueTask LogScheduledTaskSkippedAsync(ScheduledTask scheduledTask);

    ValueTask LogScheduledTasksExecutedAsync(int scheduledTaskCount);

    ScheduledTask Get(int scheduledTaskId);

    IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false);

    ScheduledTask[] GetDueScheduledTasks(DateTimeOffset currentDateTime);

    ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newScheduledTask);

    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedScheduledTask);

    ValueTask DeleteAsync(int scheduledTaskId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdateScheduledTask(IEnumerable<ScheduledTask> items);

    ValueTask DeleteAllScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems);

    ValueTask<ScheduledTask> ExecuteScheduledTaskAsync(
        int scheduledTaskId,
        bool incrementNextExecution = true);
}