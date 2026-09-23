// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowEventOrchestrationService(
    IWorkflowEventProcessingService processingService,
    IWorkflowEventEventProcessingService eventService,
    ILoggingBroker loggingBroker) : IWorkflowEventOrchestrationService
{
    public object CreateSingleResult<T>(IQueryable<T> queryable) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [queryable]);

            return processingService.CreateSingleResult(queryable: queryable);
        });

    public void LogError(Exception exception, string message) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [exception, message]);

            loggingBroker.LogError(
                exception: exception,
                message: message);

            return true;
        });

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
                ValidateWorkflowEventSubscriptionsOnGet(inputs: [appId, eventContext]);

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
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnAdd(inputs: [newWorkflowEvent]); return await ExecuteAddAsync(entity: newWorkflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.AddWorkflowEventAsync(newWorkflowEvent: entity);
        await eventService.RaiseWorkflowEventAddEventAsync(workflowEvent: result);
        return result;
    }

    public ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnUpdate(inputs: [updatedWorkflowEvent]); return await ExecuteUpdateAsync(entity: updatedWorkflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.UpdateWorkflowEventAsync(updatedWorkflowEvent: entity);
        await eventService.RaiseWorkflowEventUpdateEventAsync(workflowEvent: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid workflowEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEventId]); await ExecuteDeleteAsync(workflowEventId: workflowEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid workflowEventId)
    {
        WorkflowEvent entity = processingService.Get(workflowEventId: workflowEventId);
        await eventService.RaiseWorkflowEventDeleteEventAsync(workflowEvent: entity);
        await processingService.DeleteAsync(workflowEventId: workflowEventId);
    }

    public ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdateWorkflowEvent(IEnumerable<WorkflowEvent> items) =>
        TryCatch(operation: async () => { ValidateOrUpdateWorkflowEventOnAdd(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<WorkflowEvent>>> ExecuteAddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        return processingService.AddOrUpdateWorkflowEvent(items: items);
    }

    public ValueTask DeleteAllWorkflowEventAsync(IEnumerable<WorkflowEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllWorkflowEventOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        return processingService.DeleteAllWorkflowEventAsync(deletedItems: items);
    }
}