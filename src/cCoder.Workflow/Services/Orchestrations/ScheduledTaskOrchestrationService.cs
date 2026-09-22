// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class ScheduledTaskOrchestrationService(IScheduledTaskProcessingService processingService, IScheduledTaskEventProcessingService eventService) : IScheduledTaskOrchestrationService
{
    public ScheduledTask Get(int scheduledTaskId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [scheduledTaskId]); return ExecuteGet(scheduledTaskId: scheduledTaskId); });

    private ScheduledTask ExecuteGet(int scheduledTaskId)
    {
        return processingService.Get(scheduledTaskId: scheduledTaskId);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<ScheduledTask> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newScheduledTask) =>
        TryCatch(operation: async () => { ValidateScheduledTaskOnAdd(inputs: [newScheduledTask]); return await ExecuteAddAsync(entity: newScheduledTask); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteAddAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.AddScheduledTaskAsync(newScheduledTask: entity);
        await eventService.RaiseScheduledTaskAddEventAsync(scheduledTask: result);
        return result;
    }

    public ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedScheduledTask) =>
        TryCatch(operation: async () => { ValidateScheduledTaskOnUpdate(inputs: [updatedScheduledTask]); return await ExecuteUpdateAsync(entity: updatedScheduledTask); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteUpdateAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.UpdateScheduledTaskAsync(updatedScheduledTask: entity);
        await eventService.RaiseScheduledTaskUpdateEventAsync(scheduledTask: result);
        return result;
    }

    public ValueTask DeleteAsync(int scheduledTaskId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId]); await ExecuteDeleteAsync(scheduledTaskId: scheduledTaskId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int scheduledTaskId)
    {
        ScheduledTask entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == scheduledTaskId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseScheduledTaskDeleteEventAsync(scheduledTask: entity);
        await processingService.DeleteAsync(scheduledTaskId: scheduledTaskId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdateScheduledTask(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateOrUpdateScheduledTaskOnAdd(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<ScheduledTask>>> ExecuteAddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        return processingService.AddOrUpdateScheduledTask(items: items);
    }

    public ValueTask DeleteAllScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllScheduledTaskOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        return processingService.DeleteAllScheduledTaskAsync(deletedItems: items);
    }

    public ValueTask ExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId, incrementNextExecution]); await ExecuteExecuteAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution); }, isValueTask: true);

    private async ValueTask ExecuteExecuteAsync(
        int scheduledTaskId,
        bool incrementNextExecution = true)
    {
        ScheduledTask scheduledTask =
            await processingService.ExecuteScheduledTaskAsync(
                scheduledTaskId: scheduledTaskId,
                incrementNextExecution: incrementNextExecution);

        await eventService.RaiseScheduledTaskExecuteEventAsync(
            scheduledTask: scheduledTask);
    }
}