// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.Extensions.DependencyInjection;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class FlowDefinitionEventBroker(
    IServiceProvider serviceProvider)
        : IFlowDefinitionEventBroker
{
    public string GetCurrentUserId() =>
        serviceProvider.GetRequiredService<ICoreAuthInfo>().SSOUserId;

    public ValueTask RaiseFlowDefinitionAddEventAsync(EventMessage<FlowDefinition> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_definition_add", message: message);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(EventMessage<FlowDefinition> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_definition_update", message: message);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(EventMessage<FlowDefinition> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_definition_delete", message: message);
}