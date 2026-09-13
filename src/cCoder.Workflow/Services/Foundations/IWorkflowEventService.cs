// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowEventService
{
    WorkflowEvent Get(Guid workflowEventId);

    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);

    WorkflowEvent[] GetSubscriptions(int appId, string eventContext);

    int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent);

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent);

    ValueTask DeleteAsync(Guid workflowEventId);

    (int? AppId, string EventContext) PrepareDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null);

    string SerializePayload(object payload);
    bool LogWorkflowEventSubscriptionsFound(int count);
    bool LogWorkflowEventQueueFailure(WorkflowEvent workflowEvent, Exception exception);
    bool AuthorizeWorkflowEvent(WorkflowEvent workflowEvent);
}