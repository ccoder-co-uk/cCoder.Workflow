using cCoder.Data.Models.Workflow;

namespace cCoder.Data.Brokers;

public interface IWorkflowEventBroker
{
    IQueryable<WorkflowEvent> GetAllWorkflowEvents(bool ignoreFilters);
    ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent entity);
    ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent entity);
    ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent entity);
    ValueTask DeleteAllWorkflowEventsAsync(IEnumerable<WorkflowEvent> items);
    int? GetAppId(WorkflowEvent entity);
}