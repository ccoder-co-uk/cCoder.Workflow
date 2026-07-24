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
    public ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowDefinitionAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionAddEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionAddEventAsync(entity: entity);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowDefinitionUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionUpdateEventAsync(entity: entity);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowDefinitionDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity) =>
        eventService.RaiseFlowDefinitionDeleteEventAsync(entity: entity);
}