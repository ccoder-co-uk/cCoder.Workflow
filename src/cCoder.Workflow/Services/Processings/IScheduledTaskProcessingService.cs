// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IScheduledTaskProcessingService
{
    ScheduledTask Get(int scheduledTaskId);

    IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false);

    ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newEntity);

    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedEntity);

    ValueTask DeleteAsync(int scheduledTaskId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdateScheduledTask(IEnumerable<ScheduledTask> items);

    ValueTask DeleteAllScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems);

    ValueTask ExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true);
}