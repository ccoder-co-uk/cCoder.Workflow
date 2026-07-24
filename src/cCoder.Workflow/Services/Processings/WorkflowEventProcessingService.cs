// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowEventProcessingService(
    IWorkflowEventService service,
    IAuthorizationBroker authorizationBroker,
    IJsonBroker jsonBroker,
    ILogger<WorkflowEventProcessingService> logger)
        : IWorkflowEventProcessingService
{
    public (int? AppId, string EventContext) PrepareWorkflowEventDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload, eventName, appIdOverride]);

            return ExecutePrepareWorkflowEventDispatch(
                payload: payload,
                eventName: eventName,
                appIdOverride: appIdOverride);
        });

    private (int? AppId, string EventContext) ExecutePrepareWorkflowEventDispatch(
        object payload,
        string eventName,
        int? appIdOverride)
    {
        int? appId = appIdOverride ?? GetIntProperty(payload: payload, propertyName: "AppId");
        string context = GetStringProperty(payload: payload, propertyName: "Path") ?? string.Empty;
        string eventContext = $"{eventName}{context}";

        logger.LogDebug(
            message: "Workflow trigger event: AppId {AppId}, Context {EventContext}",
            args: [appId, eventContext]);

        return (appId, eventContext);
    }

    public string SerializeWorkflowEventPayload(object payload) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload]);
            return jsonBroker.Serialize(value: payload);
        });

    public ValueTask LogWorkflowEventQueueFailureAsync(
        WorkflowEvent workflowEvent,
        Exception exception) =>
        TryCatch(
            operation: () =>
            {
                ValidateInputs(inputs: [workflowEvent, exception]);

                logger.LogWarning(
                    exception: exception,
                    message: "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
                    args: [workflowEvent.Id, workflowEvent.FlowId]);

                return ValueTask.CompletedTask;
            },
            isValueTask: true);

    private static int? GetIntProperty(object payload, string propertyName) =>
        payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload) as int?
        ?? (payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload) is int value ? value : null);

    private static string GetStringProperty(object payload, string propertyName) =>
        payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload)?.ToString();

    public WorkflowEvent Get(Guid workflowEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [workflowEventId]); return ExecuteGet(workflowEventId: workflowEventId); });

    private WorkflowEvent ExecuteGet(Guid workflowEventId)
    {
        return service.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId, eventContext]); return await ExecuteGetSubscriptionsAsync(appId: appId, eventContext: eventContext); }, isValueTask: true);

    private ValueTask<WorkflowEvent[]> ExecuteGetSubscriptionsAsync(int appId, string eventContext)
    {
        WorkflowEvent[] subscriptions = service
            .GetAll(ignoreFilters: true)
            .Where(predicate: item => item.Flow.AppId == appId && item.EventContext == eventContext)
            .Include(navigationPropertyPath: item => item.Flow)
            .Include(navigationPropertyPath: item => item.ExecuteAsUser)
                .ThenInclude(navigationPropertyPath: user => user.Roles)
                    .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .ToArray();

        logger.LogDebug(message: "Found {Count} subscribers, calling ...", args: subscriptions.Length);

        return ValueTask.FromResult(result: subscriptions);
    }

    public ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent entity)
    {
        SecurityCheckEvent(workflowEvent: entity);
        return service.AddWorkflowEventAsync(newWorkflowEvent: entity);
    }

    public ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

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
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<WorkflowEvent>>> ExecuteAddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        List<Result<WorkflowEvent>> results = new List<Result<WorkflowEvent>>();

        foreach (WorkflowEvent item in items)
        {
            try
            {
                WorkflowEvent savedItem =
                    item.Id == Guid.Empty
                        ? await AddWorkflowEventAsync(newEntity: item)
                        : await UpdateWorkflowEventAsync(updatedEntity: item);

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
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        foreach (WorkflowEvent item in items)
        {
            await DeleteAsync(workflowEventId: item.Id);
        }
    }

    private void SecurityCheckEvent(WorkflowEvent workflowEvent)
    {
        int? appId = service.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent);
        authorizationBroker.Authorize(userId: workflowEvent.ExecuteAs, appId: appId, privilege: "app_admin");
    }
}