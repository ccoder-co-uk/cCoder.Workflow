using cCoder.Data.Models.Workflow;
using EventLibrary.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IWorkflowEventEventBroker
{
    ValueTask RaiseWorkflowEventAddEventAsync(EventMessage<WorkflowEvent> message);
    ValueTask RaiseWorkflowEventUpdateEventAsync(EventMessage<WorkflowEvent> message);
    ValueTask RaiseWorkflowEventDeleteEventAsync(EventMessage<WorkflowEvent> message);
}







