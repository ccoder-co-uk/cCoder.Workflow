// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Brokers;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowEventService(
    IWorkflowEventBroker workflowEventBroker,
    IAuthorizationBroker authorizationBroker,
    IJsonBroker jsonBroker,
    ILoggingBroker loggingBroker
) : IWorkflowEventService
{
    public (int? AppId, string EventContext) PrepareDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload, eventName, appIdOverride]);
            int? appId = appIdOverride ?? GetIntProperty(payload: payload, propertyName: "AppId");
            string context = GetStringProperty(payload: payload, propertyName: "Path") ?? string.Empty;
            string eventContext = $"{eventName}{context}";

            loggingBroker.LogDebug(
                message: "Workflow trigger event: AppId {AppId}, Context {EventContext}",
                args: [appId, eventContext]);

            return (appId, eventContext);
        });

    public string SerializePayload(object payload) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [payload]);
            return jsonBroker.Serialize(value: payload);
        });

    public bool LogWorkflowEventSubscriptionsFound(int count) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [count]);

            loggingBroker.LogDebug(
                message: "Found {Count} subscribers, calling ...",
                args: count);

            return true;
        });

    public bool LogWorkflowEventQueueFailure(
        WorkflowEvent workflowEvent,
        Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [workflowEvent, exception]);

            loggingBroker.LogWarning(
                exception: exception,
                message: "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
                args: [workflowEvent.Id, workflowEvent.FlowId]);

            return true;
        });

    public bool AuthorizeWorkflowEvent(WorkflowEvent workflowEvent) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [workflowEvent]);

            int? appId = ExecuteGetAppIdForWorkflowEvent(
                workflowEvent: workflowEvent);

            authorizationBroker.Authorize(
                userId: workflowEvent.ExecuteAs,
                appId: appId,
                privilege: "app_admin");

            return true;
        });

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
        WorkflowEvent workflowEvent = GetAll()
            .FirstOrDefault(predicate: i => i.Id == workflowEventId);

        if (workflowEvent is not null)
        {
            return workflowEvent;
        }

        WorkflowEvent unrestrictedWorkflowEvent = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == workflowEventId);

        if (unrestrictedWorkflowEvent is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return workflowEventBroker
                .SelectAllWorkflowEventsIgnoringQueryFilters();
        }

        return workflowEventBroker.SelectAllWorkflowEvents();
    }

    public WorkflowEvent[] GetSubscriptions(int appId, string eventContext) =>
        TryCatch(operation: () =>
        {
            ValidateSubscriptionsOnGet(inputs: [appId, eventContext]);

            return workflowEventBroker.SelectWorkflowEventSubscriptions(
                appId: appId,
                eventContext: eventContext);
        });

    public int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        TryCatch(operation: () => { ValidateAppIdForWorkflowEventOnGet(inputs: [workflowEvent]); return ExecuteGetAppIdForWorkflowEvent(workflowEvent: workflowEvent); });

    private int? ExecuteGetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        workflowEventBroker.SelectAppId(workflowEvent: workflowEvent);

    public ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnAdd(inputs: [newWorkflowEvent]); return await ExecuteAddAsync(workflowEvent: newWorkflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId: workflowEventBroker.SelectAppId(workflowEvent: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_create"
        );

        WorkflowEvent newWorkflowEvent = CreateStorageWorkflowEvent(item: workflowEvent);

        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newWorkflowEvent.CreatedOn = now;
        newWorkflowEvent.CreatedBy = currentUserId;

        WorkflowEvent result = await workflowEventBroker.AddWorkflowEventAsync(newWorkflowEvent: newWorkflowEvent);
        workflowEvent.Id = result.Id;
        workflowEvent.Type = result.Type;
        workflowEvent.EventContext = result.EventContext;
        workflowEvent.CreatedBy = result.CreatedBy;
        workflowEvent.CreatedOn = result.CreatedOn;
        workflowEvent.FlowId = result.FlowId;
        workflowEvent.ExecuteAs = result.ExecuteAs;
        return workflowEvent;
    }

    public ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent) =>
        TryCatch(operation: async () => { ValidateWorkflowEventOnUpdate(inputs: [updatedWorkflowEvent]); return await ExecuteUpdateAsync(workflowEvent: updatedWorkflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId: workflowEventBroker.SelectAppId(workflowEvent: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_update"
        );

        WorkflowEvent updateWorkflowEvent = CreateStorageWorkflowEvent(item: workflowEvent);

        WorkflowEvent result = await workflowEventBroker.UpdateWorkflowEventAsync(
updatedWorkflowEvent: updateWorkflowEvent
        );

        workflowEvent.Id = result.Id;
        workflowEvent.Type = result.Type;
        workflowEvent.EventContext = result.EventContext;
        workflowEvent.CreatedBy = result.CreatedBy;
        workflowEvent.CreatedOn = result.CreatedOn;
        workflowEvent.FlowId = result.FlowId;
        workflowEvent.ExecuteAs = result.ExecuteAs;
        return workflowEvent;
    }

    public ValueTask DeleteAsync(Guid workflowEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEventId]); await ExecuteDeleteAsync(workflowEventId: workflowEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid workflowEventId)
    {
        WorkflowEvent workflowEvent = Get(workflowEventId: workflowEventId);

        authorizationBroker.Authorize(
appId: workflowEventBroker.SelectAppId(workflowEvent: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_delete"
        );

        _ = await workflowEventBroker.DeleteWorkflowEventAsync(
deletedWorkflowEvent: CreateStorageWorkflowEvent(item: workflowEvent)
        );
    }

    private static WorkflowEvent CreateStorageWorkflowEvent(WorkflowEvent item) =>
        item == null
            ? null
            : new()
            {
                Id = item.Id,
                Type = item.Type,
                EventContext = item.EventContext,
                CreatedBy = item.CreatedBy,
                CreatedOn = item.CreatedOn,
                FlowId = item.FlowId,
                Flow = item.Flow,
                ExecuteAs = item.ExecuteAs,
                ExecuteAsUser = item.ExecuteAsUser,
            };
}