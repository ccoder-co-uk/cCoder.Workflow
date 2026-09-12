// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowDefinitionEventProcessingService(IFlowDefinitionEventService eventService) : IFlowDefinitionEventProcessingService
{
    public ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); await ExecuteRaiseFlowDefinitionAddEventAsync(entity: flowDefinition); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionAddEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionAddEventAsync(flowDefinition: entity);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); await ExecuteRaiseFlowDefinitionUpdateEventAsync(entity: flowDefinition); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionUpdateEventAsync(flowDefinition: entity);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); await ExecuteRaiseFlowDefinitionDeleteEventAsync(entity: flowDefinition); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionDeleteEventAsync(flowDefinition: entity);
}