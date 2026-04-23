using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class WorkflowEventEventBroker(IEventHub eventHub) : IWorkflowEventEventBroker
{
    public ValueTask RaiseWorkflowEventAddEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync("workflow_add", message);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync("workflow_update", message);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(EventMessage<WorkflowEvent> message) =>
        eventHub.RaiseEventAsync("workflow_delete", message);
}







