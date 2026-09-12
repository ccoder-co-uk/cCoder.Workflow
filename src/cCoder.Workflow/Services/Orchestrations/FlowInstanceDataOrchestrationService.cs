// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class FlowInstanceDataOrchestrationService(
    IFlowInstanceDataProcessingService processingService,
    IFlowInstanceDataEventProcessingService eventService)
        : IFlowInstanceDataOrchestrationService
{
    public FlowInstanceData Get(Guid flowInstanceDataId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowInstanceDataId]); return ExecuteGet(flowInstanceDataId: flowInstanceDataId); });

    private FlowInstanceData ExecuteGet(Guid flowInstanceDataId)
    {
        return processingService.Get(flowInstanceDataId: flowInstanceDataId);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowInstanceData> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newFlowInstanceData]); return await ExecuteAddAsync(entity: newFlowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddFlowInstanceDataAsync(newFlowInstanceData: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(flowInstanceData: result);
        return result;
    }

    public ValueTask<FlowInstanceData> AddQueuedFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newFlowInstanceData]); return await ExecuteAddQueuedAsync(entity: newFlowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddQueuedAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddQueuedFlowInstanceDataAsync(newFlowInstanceData: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(flowInstanceData: result);
        return result;
    }

    public ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedFlowInstanceData]); return await ExecuteUpdateAsync(entity: updatedFlowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteUpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.UpdateFlowInstanceDataAsync(updatedFlowInstanceData: entity);
        await eventService.RaiseFlowInstanceDataUpdateEventAsync(flowInstanceData: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteDeleteAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid flowInstanceDataId)
    {
        FlowInstanceData entity = processingService.Get(flowInstanceDataId: flowInstanceDataId);
        await eventService.RaiseFlowInstanceDataDeleteEventAsync(flowInstanceData: entity);
        await processingService.DeleteAsync(flowInstanceDataId: flowInstanceDataId);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdateFlowInstanceData(IEnumerable<FlowInstanceData> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<FlowInstanceData>>> ExecuteAddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        return processingService.AddOrUpdateFlowInstanceData(items: items);
    }

    public ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        return processingService.DeleteAllFlowInstanceDataAsync(deletedItems: items);
    }
}