// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class ScheduledTaskProcessingService(
    IScheduledTaskService service)
    : IScheduledTaskProcessingService
{
    public ScheduledTask Get(int scheduledTaskId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [scheduledTaskId]); return ExecuteGet(scheduledTaskId: scheduledTaskId); });

    private ScheduledTask ExecuteGet(int scheduledTaskId)
    {
        return service.Get(scheduledTaskId: scheduledTaskId);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<ScheduledTask> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<ScheduledTask> ExecuteScheduledTaskAsync(
        int scheduledTaskId,
        bool incrementNextExecution = true) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId, incrementNextExecution]); return await ExecuteExecuteAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution); }, isValueTask: true);

    private ValueTask<ScheduledTask> ExecuteExecuteAsync(
        int scheduledTaskId,
        bool incrementNextExecution = true) =>
        service.MarkExecutedAsync(
            scheduledTaskId: scheduledTaskId,
            incrementNextExecution: incrementNextExecution);

    public ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<ScheduledTask> ExecuteAddAsync(ScheduledTask entity) =>
        service.AddScheduledTaskAsync(newScheduledTask: entity);

    public ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private ValueTask<ScheduledTask> ExecuteUpdateAsync(ScheduledTask entity) =>
        service.UpdateScheduledTaskAsync(updatedScheduledTask: entity);

    public ValueTask DeleteAsync(int scheduledTaskId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId]); await ExecuteDeleteAsync(scheduledTaskId: scheduledTaskId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(int scheduledTaskId)
    {
        return service.DeleteAsync(scheduledTaskId: scheduledTaskId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdateScheduledTask(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<ScheduledTask>>> ExecuteAddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        List<Result<ScheduledTask>> results = new List<Result<ScheduledTask>>();

        foreach (ScheduledTask item in items)
        {
            try
            {
                ScheduledTask savedItem =
                    item.Id == 0
                        ? await AddScheduledTaskAsync(newEntity: item)
                        : await UpdateScheduledTaskAsync(updatedEntity: item);

                results.Add(item: new Result<ScheduledTask>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<ScheduledTask>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask DeleteAllScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        foreach (ScheduledTask item in items)
        {
            await DeleteAsync(scheduledTaskId: item.Id);
        }
    }
}