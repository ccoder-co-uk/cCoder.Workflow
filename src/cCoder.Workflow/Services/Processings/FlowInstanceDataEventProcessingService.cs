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
    public ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceData]); await ExecuteRaiseFlowInstanceDataAddEventAsync(entity: flowInstanceData); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataAddEventAsync(flowInstanceData: entity);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceData]); await ExecuteRaiseFlowInstanceDataUpdateEventAsync(entity: flowInstanceData); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataUpdateEventAsync(flowInstanceData: entity);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceData]); await ExecuteRaiseFlowInstanceDataDeleteEventAsync(entity: flowInstanceData); }, isValueTask: true);

    private ValueTask ExecuteRaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataDeleteEventAsync(flowInstanceData: entity);
}