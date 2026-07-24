// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IWorkflowEventProcessingService
{
    (int? AppId, string EventContext) PrepareWorkflowEventDispatch(
        object payload,
        string eventName,
        int? appIdOverride = null);

    string SerializeWorkflowEventPayload(object payload);

    ValueTask LogWorkflowEventQueueFailureAsync(
        WorkflowEvent workflowEvent,
        Exception exception);

    WorkflowEvent Get(Guid workflowEventId);

    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);

    ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext);

    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity);

    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity);

    ValueTask DeleteAsync(Guid workflowEventId);

    ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdateWorkflowEvent(IEnumerable<WorkflowEvent> items);

    ValueTask DeleteAllWorkflowEventAsync(IEnumerable<WorkflowEvent> deletedItems);
}