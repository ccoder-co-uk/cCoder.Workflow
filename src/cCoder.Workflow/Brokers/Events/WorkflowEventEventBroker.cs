// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class WorkflowEventEventBroker(IEventHub eventHub) : IWorkflowEventEventBroker
{
    public ValueTask RaiseWorkflowEventAddEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync(name: "workflow_add", message: message);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync(name: "workflow_update", message: message);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync(name: "workflow_delete", message: message);
}