// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface IWorkflowEventOrchestrationService
{
    WorkflowEvent Get(Guid workflowEventId);

    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity);

    ValueTask DeleteAsync(Guid workflowEventId);

    ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdateWorkflowEvent(IEnumerable<WorkflowEvent> items);

    ValueTask DeleteAllWorkflowEventAsync(IEnumerable<WorkflowEvent> deletedItems);
}