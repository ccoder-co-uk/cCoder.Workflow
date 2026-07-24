// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class FlowDefinitionEventBroker(IEventHub eventHub) : IFlowDefinitionEventBroker
{
    public ValueTask RaiseFlowDefinitionAddEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync(name: "flow_definition_add", message: message);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync(name: "flow_definition_update", message: message);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(EventMessage<FlowDefinition> message) =>
        eventHub.RaiseEventAsync(name: "flow_definition_delete", message: message);
}