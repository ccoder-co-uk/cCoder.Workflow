// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IWorkflowEventEventBroker
{
    string GetCurrentUserId();
    ValueTask RaiseWorkflowEventAddEventAsync(EventMessage<WorkflowEvent> message);
    ValueTask RaiseWorkflowEventUpdateEventAsync(EventMessage<WorkflowEvent> message);
    ValueTask RaiseWorkflowEventDeleteEventAsync(EventMessage<WorkflowEvent> message);
}