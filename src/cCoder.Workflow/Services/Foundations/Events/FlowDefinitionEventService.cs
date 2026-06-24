using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class FlowDefinitionEventService(
    IFlowDefinitionEventBroker flowDefinitionEventBroker,
    ICoreAuthInfo authInfo
) : IFlowDefinitionEventService
{
    public async ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity)
    {
        EventMessage<FlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionAddEventAsync(message);
    }

    public async ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity)
    {
        EventMessage<FlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionUpdateEventAsync(message);
    }

    public async ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity)
    {
        EventMessage<FlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionDeleteEventAsync(message);
    }
}









