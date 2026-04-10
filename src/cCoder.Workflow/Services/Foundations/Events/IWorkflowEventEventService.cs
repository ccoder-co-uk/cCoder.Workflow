using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

public interface IWorkflowEventEventService
{
    ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity);
    ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity);
    ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity);
}








