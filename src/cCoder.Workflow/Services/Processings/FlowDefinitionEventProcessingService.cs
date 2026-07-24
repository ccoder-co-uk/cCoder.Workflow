// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class FlowDefinitionEventProcessingService(IFlowDefinitionEventService eventService) : IFlowDefinitionEventProcessingService
{
    public ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity) => eventService.RaiseFlowDefinitionAddEventAsync(entity);

    public ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity) => eventService.RaiseFlowDefinitionUpdateEventAsync(entity);

    public ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity) => eventService.RaiseFlowDefinitionDeleteEventAsync(entity);
}