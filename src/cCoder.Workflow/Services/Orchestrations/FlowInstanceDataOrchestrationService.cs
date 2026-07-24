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

    public ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddQueuedAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddQueuedAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.AddQueuedAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteUpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseFlowInstanceDataUpdateEventAsync(entity: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteDeleteAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid flowInstanceDataId)
    {
        FlowInstanceData entity = processingService.Get(flowInstanceDataId: flowInstanceDataId);
        await eventService.RaiseFlowInstanceDataDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(flowInstanceDataId: flowInstanceDataId);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<FlowInstanceData>>> ExecuteAddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}