using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Exposures.EventHandlers;

internal class WorkflowEventHandlers(IEventHandlerService eventHandlerService)
    : IWorkflowEventHandlers
{
    public void ListenToAllEvents() => eventHandlerService.ListenToAllEvents();
}


