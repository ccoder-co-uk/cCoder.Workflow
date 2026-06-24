using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowInstanceDataOrchestrationService(
    IFlowInstanceDataProcessingService processingService, 
    IFlowInstanceDataEventProcessingService eventService) 
        : IFlowInstanceDataOrchestrationService
{
    public FlowInstanceData Get(Guid id)
    {
        return processingService.Get(id);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddAsync(entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(result);
        return result;
    }

    public async ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddQueuedAsync(entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(result);
        return result;
    }

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.UpdateAsync(entity);
        await eventService.RaiseFlowInstanceDataUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowInstanceData entity = processingService.Get(id);
        await eventService.RaiseFlowInstanceDataDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}
