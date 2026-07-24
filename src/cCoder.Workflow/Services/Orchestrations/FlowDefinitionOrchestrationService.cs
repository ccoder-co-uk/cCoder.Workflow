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
    public bool AuthorizeFlowDefinitionExecution(string userId, int? appId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [userId, appId]);
            return processingService.AuthorizeFlowDefinitionExecution(userId: userId, appId: appId);
        });

    public FlowInstanceData CreateFlowDefinitionQueuedFlowInstanceData(
        FlowDefinition flowDefinition,
        string caller,
        string args) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [flowDefinition, caller, args]);

            return processingService.CreateFlowDefinitionQueuedFlowInstanceData(
                flowDefinition: flowDefinition,
                caller: caller,
                args: args);
        });

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

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddFlowDefinitionAsync(newEntity: entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateFlowDefinitionAsync(updatedEntity: entity);
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

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdateFlowDefinition(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<FlowDefinition>>> ExecuteAddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdateFlowDefinition(items: items);
    }

    public ValueTask DeleteAllFlowDefinitionAsync(IEnumerable<FlowDefinition> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllFlowDefinitionAsync(deletedItems: items);
    }
}