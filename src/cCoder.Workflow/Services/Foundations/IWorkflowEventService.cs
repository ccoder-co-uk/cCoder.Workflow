using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface IWorkflowEventService
{
    WorkflowEvent Get(Guid id);
    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);
    int? GetAppIdForWorkflowEvent(WorkflowEvent workflowEvent);
    ValueTask<WorkflowEvent> AddAsync(WorkflowEvent workflowEvent);
    ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent workflowEvent);
    ValueTask DeleteAsync(Guid id);
}








