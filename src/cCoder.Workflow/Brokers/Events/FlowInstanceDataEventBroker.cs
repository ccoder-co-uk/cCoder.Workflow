using cCoder.Data.Models.Workflow;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.Workflow.Brokers.Events;

public class FlowInstanceDataEventBroker(IEventHub eventHub) : IFlowInstanceDataEventBroker
{
    public ValueTask RaiseFlowInstanceDataAddEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync("flow_instance_data_add", message);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync("flow_instance_data_update", message);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(EventMessage<FlowInstanceData> message) =>
        eventHub.RaiseEventAsync("flow_instance_data_delete", message);
}







