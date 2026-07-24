// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface IWorkflowEventService
{
    WorkflowEvent Get(Guid workflowEventId);

    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);

    int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent);

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent);

    ValueTask DeleteAsync(Guid workflowEventId);
}