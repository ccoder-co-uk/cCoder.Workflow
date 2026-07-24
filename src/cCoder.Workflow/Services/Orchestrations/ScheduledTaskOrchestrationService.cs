// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class ScheduledTaskOrchestrationService(IScheduledTaskProcessingService processingService, IScheduledTaskEventProcessingService eventService) : IScheduledTaskOrchestrationService
{
    public ScheduledTask Get(int id)
    {
        return processingService.Get(id:id);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters:ignoreFilters);
    }

    public async ValueTask<ScheduledTask> AddAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.AddAsync(entity:entity);
        await eventService.RaiseScheduledTaskAddEventAsync(entity:result);
        return result;
    }

    public async ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.UpdateAsync(entity:entity);
        await eventService.RaiseScheduledTaskUpdateEventAsync(entity:result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ScheduledTask entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(predicate:item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseScheduledTaskDeleteEventAsync(entity:entity);
        await processingService.DeleteAsync(id:id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId:appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        return processingService.AddOrUpdate(items:items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        return processingService.DeleteAllAsync(items:items);
    }

    public ValueTask ExecuteAsync(int id, bool incrementNextExecution = true)
    {
        return processingService.ExecuteAsync(id:id, incrementNextExecution:incrementNextExecution);
    }
}