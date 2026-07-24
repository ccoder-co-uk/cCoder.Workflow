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
    public ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowInstanceData> message = CreateMessage(entity: entity);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowInstanceData> message = CreateMessage(entity: entity);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowInstanceData> message = CreateMessage(entity: entity);

            await flowInstanceDataEventBroker.RaiseFlowInstanceDataDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<FlowInstanceData> CreateMessage(FlowInstanceData entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = flowInstanceDataEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}