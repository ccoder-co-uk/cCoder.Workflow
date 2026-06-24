using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;

namespace cCoder.Workflow.Services.Processings;

internal class WorkflowEventEventProcessingService(IWorkflowEventEventService eventService) 
    : IWorkflowEventEventProcessingService
{
    public ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity) => 
        eventService.RaiseWorkflowEventAddEventAsync(entity);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity) => 
        eventService.RaiseWorkflowEventUpdateEventAsync(entity);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity) => 
        eventService.RaiseWorkflowEventDeleteEventAsync(entity);
}