// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Data.Brokers;

public interface IWorkflowEventBroker
{
    IQueryable<WorkflowEvent> SelectAllWorkflowEvents();

    IQueryable<WorkflowEvent> SelectAllWorkflowEventsIgnoringQueryFilters();

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity);

    ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent deletedEntity);

    int? SelectAppId(WorkflowEvent entity);
}