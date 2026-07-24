// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IFlowDefinitionEventBroker
{
    string GetCurrentUserId();
    ValueTask RaiseFlowDefinitionAddEventAsync(EventMessage<FlowDefinition> message);
    ValueTask RaiseFlowDefinitionUpdateEventAsync(EventMessage<FlowDefinition> message);
    ValueTask RaiseFlowDefinitionDeleteEventAsync(EventMessage<FlowDefinition> message);
}