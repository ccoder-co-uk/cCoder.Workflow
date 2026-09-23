// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowEventProcessingService(
    IWorkflowEventService service)
        : IWorkflowEventProcessingService
{
    public object CreateSingleResult<T>(IQueryable<T> queryable) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [queryable]);

            return service.CreateSingleResult(queryable: queryable);
        });

    public (int? AppId, string EventContext) PrepareWorkflowEventDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload, eventName, appIdOverride]);

            return service.PrepareDispatch(
                payload: payload,
                eventName: eventName,
                appIdOverride: appIdOverride);
        });

    public string SerializeWorkflowEventPayload(object payload) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload]);
            return service.SerializePayload(payload: payload);
        });

    public ValueTask LogWorkflowEventQueueFailureAsync(
        WorkflowEvent workflowEvent,
        Exception exception) =>
        TryCatch(
            operation: () =>
            {
                ValidateInputs(inputs: [workflowEvent, exception]);

                _ = service.LogWorkflowEventQueueFailure(
                    workflowEvent: workflowEvent,
                    exception: exception);

                return ValueTask.CompletedTask;
            },
            isValueTask: true);

    public WorkflowEvent Get(Guid workflowEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [workflowEventId]); return ExecuteGet(workflowEventId: workflowEventId); });

    private WorkflowEvent ExecuteGet(Guid workflowEventId)
    {
        return service.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext) =>
        TryCatch(operation: async () => { ValidateSubscriptionsOnGet(inputs: [appId, eventContext]); return await ExecuteGetSubscriptionsAsync(appId: appId, eventContext: eventContext); }, isValueTask: true);

    private ValueTask<WorkflowEvent[]> ExecuteGetSubscriptionsAsync(int appId, string eventContext)
    {
        WorkflowEvent[] subscriptions = service.GetSubscriptions(
            appId: appId,
            eventContext: eventContext);

        _ = service.LogWorkflowEventSubscriptionsFound(
            count: subscriptions.Length);

        return ValueTask.FromResult(result: subscriptions);
    }

    public ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnAdd(inputs: [newWorkflowEvent]); return await ExecuteAddAsync(entity: newWorkflowEvent); }, isValueTask: true);

    private ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent entity)
    {
        SecurityCheckEvent(workflowEvent: entity);
        return service.AddWorkflowEventAsync(newWorkflowEvent: entity);
    }

    public ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnUpdate(inputs: [updatedWorkflowEvent]); return await ExecuteUpdateAsync(entity: updatedWorkflowEvent); }, isValueTask: true);

    private ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent entity)
    {
        SecurityCheckEvent(workflowEvent: entity);
        return service.UpdateWorkflowEventAsync(updatedWorkflowEvent: entity);
    }

    public ValueTask DeleteAsync(Guid workflowEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEventId]); await ExecuteDeleteAsync(workflowEventId: workflowEventId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(Guid workflowEventId)
    {
        return service.DeleteAsync(workflowEventId: workflowEventId);
    }

    public ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdateWorkflowEvent(IEnumerable<WorkflowEvent> items) =>
        TryCatch(operation: async () => { ValidateOrUpdateWorkflowEventOnAdd(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<WorkflowEvent>>> ExecuteAddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        List<Result<WorkflowEvent>> results = new List<Result<WorkflowEvent>>();

        foreach (WorkflowEvent item in items)
        {
            try
            {
                WorkflowEvent savedItem =
                    item.Id == Guid.Empty
                        ? await AddWorkflowEventAsync(newWorkflowEvent: item)
                        : await UpdateWorkflowEventAsync(updatedWorkflowEvent: item);

                results.Add(item: new Result<WorkflowEvent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<WorkflowEvent>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask DeleteAllWorkflowEventAsync(IEnumerable<WorkflowEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllWorkflowEventOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        foreach (WorkflowEvent item in items)
        {
            await DeleteAsync(workflowEventId: item.Id);
        }
    }

    private void SecurityCheckEvent(WorkflowEvent workflowEvent)
    {
        _ = service.AuthorizeWorkflowEvent(workflowEvent: workflowEvent);
    }
}