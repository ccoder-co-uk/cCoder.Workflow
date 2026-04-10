using cCoder.Data.Models.Workflow;
using EventLibrary.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IFlowDefinitionEventBroker
{
    ValueTask RaiseFlowDefinitionAddEventAsync(EventMessage<FlowDefinition> message);
    ValueTask RaiseFlowDefinitionUpdateEventAsync(EventMessage<FlowDefinition> message);
    ValueTask RaiseFlowDefinitionDeleteEventAsync(EventMessage<FlowDefinition> message);
}







