// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

internal interface IFlowDefinitionEventProcessingService
{
    ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition flowDefinition);

    ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition flowDefinition);

    ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition flowDefinition);
}