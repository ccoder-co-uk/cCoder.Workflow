// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Data.Brokers;

public interface IWorkflowEventBroker
{
    IQueryable<WorkflowEvent> SelectAllWorkflowEvents();

    IQueryable<WorkflowEvent> SelectAllWorkflowEventsIgnoringQueryFilters();

    WorkflowEvent[] SelectWorkflowEventSubscriptions(int appId, string eventContext);

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent);

    ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent deletedWorkflowEvent);

    int? SelectAppId(WorkflowEvent workflowEvent);
}