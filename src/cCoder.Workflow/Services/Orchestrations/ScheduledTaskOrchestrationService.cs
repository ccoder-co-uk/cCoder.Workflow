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
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<ScheduledTask> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<ScheduledTask> AddAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteAddAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseScheduledTaskAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<ScheduledTask> ExecuteUpdateAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseScheduledTaskUpdateEventAsync(entity: result);
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

        await eventService.RaiseScheduledTaskDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(scheduledTaskId: scheduledTaskId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<ScheduledTask>>> ExecuteAddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }

    public ValueTask ExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId, incrementNextExecution]); await ExecuteExecuteAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution); }, isValueTask: true);

    private ValueTask ExecuteExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true)
    {
        return processingService.ExecuteAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution);
    }
}