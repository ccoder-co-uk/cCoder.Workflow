// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

internal interface IFlowDefinitionEventProcessingService
{
    ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition flowDefinition);

    ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition flowDefinition);

    ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition flowDefinition);
}