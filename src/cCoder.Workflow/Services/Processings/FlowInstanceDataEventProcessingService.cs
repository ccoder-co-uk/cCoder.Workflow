// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class FlowInstanceDataEventProcessingService(IFlowInstanceDataEventService eventService) : IFlowInstanceDataEventProcessingService
{
    public ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataAddEventAsync(entity: entity);

    public ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataUpdateEventAsync(entity: entity);

    public ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity) =>
        eventService.RaiseFlowInstanceDataDeleteEventAsync(entity: entity);
}