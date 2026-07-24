// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Brokers;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowEventService(
    IWorkflowEventBroker workflowEventBroker,
    IAuthorizationBroker authorizationBroker
) : IWorkflowEventService
{
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

    public int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        TryCatch(operation: () => { ValidateAppIdForWorkflowEventOnGet(inputs: [workflowEvent]); return ExecuteGetAppIdForWorkflowEvent(workflowEvent: workflowEvent); });

    private int? ExecuteGetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        workflowEventBroker.SelectAppId(entity: workflowEvent);

    public ValueTask<WorkflowEvent> AddAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEvent]); return await ExecuteAddAsync(workflowEvent: workflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId: workflowEventBroker.SelectAppId(entity: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_create"
        );

        WorkflowEvent newWorkflowEvent = CreateStorageWorkflowEvent(item: workflowEvent);

        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newWorkflowEvent.CreatedOn = now;
        newWorkflowEvent.CreatedBy = currentUserId;

        WorkflowEvent result = await workflowEventBroker.AddWorkflowEventAsync(entity: newWorkflowEvent);
        workflowEvent.Id = result.Id;
        workflowEvent.Type = result.Type;
        workflowEvent.EventContext = result.EventContext;
        workflowEvent.CreatedBy = result.CreatedBy;
        workflowEvent.CreatedOn = result.CreatedOn;
        workflowEvent.FlowId = result.FlowId;
        workflowEvent.ExecuteAs = result.ExecuteAs;
        return workflowEvent;
    }

    public ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEvent]); return await ExecuteUpdateAsync(workflowEvent: workflowEvent); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId: workflowEventBroker.SelectAppId(entity: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_update"
        );

        WorkflowEvent updateWorkflowEvent = CreateStorageWorkflowEvent(item: workflowEvent);

        WorkflowEvent result = await workflowEventBroker.UpdateWorkflowEventAsync(
entity: updateWorkflowEvent
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
appId: workflowEventBroker.SelectAppId(entity: workflowEvent),
privilege: $"{nameof(WorkflowEvent)}_delete"
        );

        _ = await workflowEventBroker.DeleteWorkflowEventAsync(
entity: CreateStorageWorkflowEvent(item: workflowEvent)
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