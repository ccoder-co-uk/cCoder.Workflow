// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowInstanceDataEventProcessingService(IFlowInstanceDataEventService eventService) : IFlowInstanceDataEventProcessingService
{
    public ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowInstanceDataAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataAddEventAsync(entity: entity);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowInstanceDataUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataUpdateEventAsync(entity: entity);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseFlowInstanceDataDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataDeleteEventAsync(entity: entity);
}