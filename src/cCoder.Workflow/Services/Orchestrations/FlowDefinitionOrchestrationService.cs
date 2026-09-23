// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class FlowDefinitionOrchestrationService(
    IFlowDefinitionProcessingService processingService,
    IFlowDefinitionEventProcessingService eventService)
        : IFlowDefinitionOrchestrationService
{
    public FlowDefinition Get(Guid flowDefinitionId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowDefinitionId]); return ExecuteGet(flowDefinitionId: flowDefinitionId); });

    private FlowDefinition ExecuteGet(Guid flowDefinitionId)
    {
        return processingService.Get(flowDefinitionId: flowDefinitionId);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowDefinition> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition) =>
        TryCatch(operation: async () => { ValidateFlowDefinitionOnAdd(inputs: [newFlowDefinition]); return await ExecuteAddAsync(entity: newFlowDefinition); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddFlowDefinitionAsync(newFlowDefinition: entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(flowDefinition: result);
        return result;
    }

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition) =>
        TryCatch(operation: async () => { ValidateFlowDefinitionOnUpdate(inputs: [updatedFlowDefinition]); return await ExecuteUpdateAsync(entity: updatedFlowDefinition); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateFlowDefinitionAsync(updatedFlowDefinition: entity);
        await eventService.RaiseFlowDefinitionUpdateEventAsync(flowDefinition: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid flowDefinitionId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinitionId]); await ExecuteDeleteAsync(flowDefinitionId: flowDefinitionId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid flowDefinitionId)
    {
        FlowDefinition entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == flowDefinitionId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseFlowDefinitionDeleteEventAsync(flowDefinition: entity);
        await processingService.DeleteAsync(flowDefinitionId: flowDefinitionId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdateFlowDefinition(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateOrUpdateFlowDefinitionOnAdd(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<FlowDefinition>>> ExecuteAddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdateFlowDefinition(items: items);
    }

    public ValueTask DeleteAllFlowDefinitionAsync(IEnumerable<FlowDefinition> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllFlowDefinitionOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllFlowDefinitionAsync(deletedItems: items);
    }
}