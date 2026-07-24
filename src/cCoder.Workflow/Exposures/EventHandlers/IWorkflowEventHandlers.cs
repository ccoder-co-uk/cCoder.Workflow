// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Exposures.EventHandlers;

public interface IWorkflowEventHandlers
{
    void ListenToAllEvents();

    void ListenToScheduledTaskExecuteEvents();

    void ListenToQueuedFlowInstanceExecuteEvents();
}