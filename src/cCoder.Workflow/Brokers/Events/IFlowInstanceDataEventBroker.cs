using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IFlowInstanceDataEventBroker
{
    ValueTask RaiseFlowInstanceDataAddEventAsync(EventMessage<FlowInstanceData> message);
    ValueTask RaiseFlowInstanceDataUpdateEventAsync(EventMessage<FlowInstanceData> message);
    ValueTask RaiseFlowInstanceDataDeleteEventAsync(EventMessage<FlowInstanceData> message);
}







