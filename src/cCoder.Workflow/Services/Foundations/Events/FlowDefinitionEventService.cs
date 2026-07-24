// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class FlowDefinitionEventService(
    IFlowDefinitionEventBroker flowDefinitionEventBroker)
        : IFlowDefinitionEventService
{
    public ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowDefinition> message = CreateFlowDefinitionMessage(entity: entity);

            await flowDefinitionEventBroker.RaiseFlowDefinitionAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowDefinition> message = CreateFlowDefinitionMessage(entity: entity);

            await flowDefinitionEventBroker.RaiseFlowDefinitionUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<FlowDefinition> message = CreateFlowDefinitionMessage(entity: entity);

            await flowDefinitionEventBroker.RaiseFlowDefinitionDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<FlowDefinition> CreateFlowDefinitionMessage(FlowDefinition entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = flowDefinitionEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}