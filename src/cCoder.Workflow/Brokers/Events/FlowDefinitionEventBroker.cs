using cCoder.Data.Models.Workflow;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.Workflow.Brokers.Events;

public class FlowDefinitionEventBroker(IEventHub eventHub) : IFlowDefinitionEventBroker
{
    public ValueTask RaiseFlowDefinitionAddEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync("flow_definition_add", message);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync("flow_definition_update", message);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync("flow_definition_delete", message);
}







