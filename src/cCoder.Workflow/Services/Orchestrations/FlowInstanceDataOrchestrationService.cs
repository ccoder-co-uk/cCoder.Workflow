// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowInstanceDataOrchestrationService(
    IFlowInstanceDataProcessingService processingService,
    IFlowInstanceDataEventProcessingService eventService)
        : IFlowInstanceDataOrchestrationService
{
    public FlowInstanceData Get(Guid flowInstanceDataId)
    {
        return processingService.Get(flowInstanceDataId: flowInstanceDataId);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddQueuedAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid flowInstanceDataId)
    {
        FlowInstanceData entity = processingService.Get(flowInstanceDataId: flowInstanceDataId);
        await eventService.RaiseFlowInstanceDataDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(flowInstanceDataId: flowInstanceDataId);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}