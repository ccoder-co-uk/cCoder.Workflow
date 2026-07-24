// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.Extensions.DependencyInjection;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class FlowInstanceDataEventBroker(
    IServiceProvider serviceProvider)
        : IFlowInstanceDataEventBroker
{
    public string GetCurrentUserId() =>
        serviceProvider.GetRequiredService<ICoreAuthInfo>().SSOUserId;

    public ValueTask RaiseFlowInstanceDataAddEventAsync(EventMessage<FlowInstanceData> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_instance_data_add", message: message);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(EventMessage<FlowInstanceData> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_instance_data_update", message: message);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(EventMessage<FlowInstanceData> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "flow_instance_data_delete", message: message);
}