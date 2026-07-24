// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Exposures.EventHandlers;

internal class WorkflowEventHandlers(IEventHandlerService eventHandlerService)
    : IWorkflowEventHandlers
{
    public void ListenToAllEvents() =>
        eventHandlerService.ListenToAllEvents();

    public void ListenToScheduledTaskExecuteEvents() =>
        eventHandlerService.ListenToScheduledTaskExecuteEvents();

    public void ListenToQueuedFlowInstanceExecuteEvents() =>
        eventHandlerService.ListenToQueuedFlowInstanceExecuteEvents();
}