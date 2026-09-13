// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Exposures;

internal sealed class FlowDefinitionEventHandler(
    IFlowDefinitionCoordinationService flowDefinitionCoordinationService)
    : IFlowDefinitionEventHandler
{
    public ValueTask HandleFlowDefinitionDeleteAsync(
        FlowDefinition flowDefinition) =>
        flowDefinitionCoordinationService.HandleFlowDefinitionDeleteAsync(
            flowDefinition: flowDefinition);
}