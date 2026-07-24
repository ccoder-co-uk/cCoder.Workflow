// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Brokers;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal class WorkflowEventService(
    IWorkflowEventBroker workflowEventBroker,
    IAuthorizationBroker authorizationBroker
) : IWorkflowEventService
{
    public WorkflowEvent Get(Guid id)
    {
        WorkflowEvent workflowEvent = GetAll().FirstOrDefault(predicate:i => i.Id == id);
        if (workflowEvent is not null)
        {
            return workflowEvent;
        }

        WorkflowEvent unrestrictedWorkflowEvent = GetAll(ignoreFilters:true).FirstOrDefault(predicate:i => i.Id == id);
        if (unrestrictedWorkflowEvent is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        workflowEventBroker.GetAllWorkflowEvents(ignoreFilters:ignoreFilters);

    public int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        workflowEventBroker.GetAppId(entity:workflowEvent);

    public async ValueTask<WorkflowEvent> AddAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId:            workflowEventBroker.GetAppId(workflowEvent),
privilege:            $"{nameof(WorkflowEvent)}_create"
        );

        WorkflowEvent newWorkflowEvent = CreateStorageWorkflowEvent(item:workflowEvent);

        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newWorkflowEvent.CreatedOn = now;
        newWorkflowEvent.CreatedBy = currentUserId;

        WorkflowEvent result = await workflowEventBroker.AddWorkflowEventAsync(entity:newWorkflowEvent);
        workflowEvent.Id = result.Id;
        workflowEvent.Type = result.Type;
        workflowEvent.EventContext = result.EventContext;
        workflowEvent.CreatedBy = result.CreatedBy;
        workflowEvent.CreatedOn = result.CreatedOn;
        workflowEvent.FlowId = result.FlowId;
        workflowEvent.ExecuteAs = result.ExecuteAs;
        return workflowEvent;
    }

    public async ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
appId:            workflowEventBroker.GetAppId(workflowEvent),
privilege:            $"{nameof(WorkflowEvent)}_update"
        );

        WorkflowEvent updateWorkflowEvent = CreateStorageWorkflowEvent(item:workflowEvent);

        WorkflowEvent result = await workflowEventBroker.UpdateWorkflowEventAsync(
entity:            updateWorkflowEvent
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

    public async ValueTask DeleteAsync(Guid id)
    {
        WorkflowEvent workflowEvent = Get(id:id);
        authorizationBroker.Authorize(
appId:            workflowEventBroker.GetAppId(workflowEvent),
privilege:            $"{nameof(WorkflowEvent)}_delete"
        );
        _ = await workflowEventBroker.DeleteWorkflowEventAsync(
entity:            CreateStorageWorkflowEvent(workflowEvent)
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