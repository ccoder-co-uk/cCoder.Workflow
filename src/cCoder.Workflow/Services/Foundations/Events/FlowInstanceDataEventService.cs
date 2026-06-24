using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;

namespace cCoder.Workflow.Services.Foundations.Events;

internal class FlowInstanceDataEventService(
    IFlowInstanceDataEventBroker flowInstanceDataEventBroker,
    ICoreAuthInfo authInfo
) : IFlowInstanceDataEventService
{
    public async ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity)
    {
        EventMessage<FlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataAddEventAsync(message);
    }

    public async ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity)
    {
        EventMessage<FlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataUpdateEventAsync(message);
    }

    public async ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity)
    {
        EventMessage<FlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataDeleteEventAsync(message);
    }
}