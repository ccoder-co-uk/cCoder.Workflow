// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Exposures;

internal sealed class FlowDefinitionEventHandler(
    IFlowDefinitionCoordinationService flowDefinitionCoordinationService)
    : IFlowDefinitionEventHandler, ICompositionExposure
{
    public ValueTask HandleFlowDefinitionDeleteAsync(
        FlowDefinition flowDefinition) =>
        flowDefinitionCoordinationService.HandleFlowDefinitionDeleteAsync(
            flowDefinition: flowDefinition);
}