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
        WorkflowEvent workflowEvent = GetAll().FirstOrDefault(i => i.Id == id);
        if (workflowEvent is not null)
            return workflowEvent;

        WorkflowEvent unrestrictedWorkflowEvent = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedWorkflowEvent is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        workflowEventBroker.GetAllWorkflowEvents(ignoreFilters);

    public int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent) =>
        workflowEventBroker.GetAppId(workflowEvent);

    public async ValueTask<WorkflowEvent> AddAsync(WorkflowEvent workflowEvent)
    {
        authorizationBroker.Authorize(
            workflowEventBroker.GetAppId(workflowEvent),
            $"{nameof(WorkflowEvent)}_create"
        );

        WorkflowEvent newWorkflowEvent = CreateStorageWorkflowEvent(workflowEvent);

        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newWorkflowEvent.CreatedOn = now;
        newWorkflowEvent.CreatedBy = currentUserId;

        WorkflowEvent result = await workflowEventBroker.AddWorkflowEventAsync(newWorkflowEvent);
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
            workflowEventBroker.GetAppId(workflowEvent),
            $"{nameof(WorkflowEvent)}_update"
        );

        WorkflowEvent updateWorkflowEvent = CreateStorageWorkflowEvent(workflowEvent);

        WorkflowEvent result = await workflowEventBroker.UpdateWorkflowEventAsync(
            updateWorkflowEvent
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
        WorkflowEvent workflowEvent = Get(id);
        authorizationBroker.Authorize(
            workflowEventBroker.GetAppId(workflowEvent),
            $"{nameof(WorkflowEvent)}_delete"
        );
        _ = await workflowEventBroker.DeleteWorkflowEventAsync(
            CreateStorageWorkflowEvent(workflowEvent)
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











