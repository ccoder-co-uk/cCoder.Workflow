// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class FlowInstanceDataEventBroker(IEventHub eventHub) : IFlowInstanceDataEventBroker
{
    public ValueTask RaiseFlowInstanceDataAddEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync(name: "flow_instance_data_add", message: message);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync(name: "flow_instance_data_update", message: message);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync(name: "flow_instance_data_delete", message: message);
}