// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowDefinitionOrchestrationService(
    IFlowDefinitionProcessingService processingService,
    IFlowDefinitionEventProcessingService eventService)
        : IFlowDefinitionOrchestrationService
{
    public FlowDefinition Get(Guid id)
    {
        return processingService.Get(id:id);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters:ignoreFilters);
    }

    public async ValueTask<FlowDefinition> AddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddAsync(entity:entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(entity:result);
        return result;
    }

    public async ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateAsync(entity:entity);
        await eventService.RaiseFlowDefinitionUpdateEventAsync(entity:result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowDefinition entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(predicate:item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseFlowDefinitionDeleteEventAsync(entity:entity);
        await processingService.DeleteAsync(id:id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId:appId);

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdate(items:items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllAsync(items:items);
    }
}