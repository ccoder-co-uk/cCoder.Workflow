using cCoder.Workflow.Models;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class ScheduledTaskOrchestrationService(IScheduledTaskProcessingService processingService, IScheduledTaskEventProcessingService eventService) : IScheduledTaskOrchestrationService
{
    public ScheduledTask Get(int id)
    {
        return processingService.Get(id);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<ScheduledTask> AddAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.AddAsync(entity);
        await eventService.RaiseScheduledTaskAddEventAsync(result);
        return result;
    }

    public async ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity)
    {
        ScheduledTask result = await processingService.UpdateAsync(entity);
        await eventService.RaiseScheduledTaskUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ScheduledTask entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseScheduledTaskDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    public ValueTask ExecuteAsync(int id, bool incrementNextExecution = true)
    {
        return processingService.ExecuteAsync(id, incrementNextExecution);
    }
}
