// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowEventOrchestrationService(IWorkflowEventProcessingService processingService, IWorkflowEventEventProcessingService eventService) : IWorkflowEventOrchestrationService
{
    public (int? AppId, string EventContext) PrepareWorkflowEventDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload, eventName, appIdOverride]);

            return processingService.PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: eventName,
                appIdOverride: appIdOverride);
        });

    public string SerializeWorkflowEventPayload(object payload) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload]);
            return processingService.SerializeWorkflowEventPayload(payload: payload);
        });

    public ValueTask<WorkflowEvent[]> GetWorkflowEventSubscriptionsAsync(
        int appId,
        string eventContext) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [appId, eventContext]);

                return await processingService.GetSubscriptionsAsync(
                    appId: appId,
                    eventContext: eventContext);
            },
            isValueTask: true);

    public ValueTask LogWorkflowEventQueueFailureAsync(
        WorkflowEvent workflowEvent,
        Exception exception) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [workflowEvent, exception]);

                await processingService.LogWorkflowEventQueueFailureAsync(
                    workflowEvent: workflowEvent,
                    exception: exception);
            },
            isValueTask: true);

    public WorkflowEvent Get(Guid workflowEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [workflowEventId]); return ExecuteGet(workflowEventId: workflowEventId); });

    private WorkflowEvent ExecuteGet(Guid workflowEventId)
    {
        return processingService.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.AddWorkflowEventAsync(newEntity: entity);
        await eventService.RaiseWorkflowEventAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.UpdateWorkflowEventAsync(updatedEntity: entity);
        await eventService.RaiseWorkflowEventUpdateEventAsync(entity: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid workflowEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEventId]); await ExecuteDeleteAsync(workflowEventId: workflowEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid workflowEventId)
    {
        WorkflowEvent entity = processingService.Get(workflowEventId: workflowEventId);
        await eventService.RaiseWorkflowEventDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(workflowEventId: workflowEventId);
    }

    public ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdateWorkflowEvent(IEnumerable<WorkflowEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<WorkflowEvent>>> ExecuteAddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        return processingService.AddOrUpdateWorkflowEvent(items: items);
    }

    public ValueTask DeleteAllWorkflowEventAsync(IEnumerable<WorkflowEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        return processingService.DeleteAllWorkflowEventAsync(deletedItems: items);
    }
}