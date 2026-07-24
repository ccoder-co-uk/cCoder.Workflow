// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations.Events;

public interface IEventHandlerService
{
    void ListenToAllEvents();

    void ListenToScheduledTaskExecuteEvents();

    void ListenToQueuedFlowInstanceExecuteEvents();
}