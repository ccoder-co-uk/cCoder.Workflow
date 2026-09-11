// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class FlowInstanceDataEventService(
    IFlowInstanceDataEventBroker flowInstanceDataEventBroker)
        : IFlowInstanceDataEventService
{
    public ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowInstanceData]);

            EventMessage<FlowInstanceData> message = CreateFlowInstanceDataMessage(entity: flowInstanceData);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowInstanceData]);

            EventMessage<FlowInstanceData> message = CreateFlowInstanceDataMessage(entity: flowInstanceData);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowInstanceData]);

            EventMessage<FlowInstanceData> message = CreateFlowInstanceDataMessage(entity: flowInstanceData);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<FlowInstanceData> CreateFlowInstanceDataMessage(FlowInstanceData entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = flowInstanceDataEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}