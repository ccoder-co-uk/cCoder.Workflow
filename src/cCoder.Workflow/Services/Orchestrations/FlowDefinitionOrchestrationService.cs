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
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowDefinition> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowDefinition> AddAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseFlowDefinitionUpdateEventAsync(entity: result);
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

        await eventService.RaiseFlowDefinitionDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(flowDefinitionId: flowDefinitionId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<FlowDefinition>>> ExecuteAddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}