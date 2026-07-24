// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Data.Brokers;

public interface IWorkflowEventBroker
{
    IQueryable<WorkflowEvent> SelectAllWorkflowEvents();

    IQueryable<WorkflowEvent> SelectAllWorkflowEventsIgnoringQueryFilters();

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent entity);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent entity);

    ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent entity);

    int? SelectAppId(WorkflowEvent entity);
}