// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Coordinations;

internal interface IFlowDefinitionManagementCoordinationService
{
    FlowDefinition GetFlowDefinition(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAllFlowDefinitions();

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition);

    ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId);

    ValueTask<string> ExecuteScriptAsync(string script);

    ValueTask<string> ReadRequestBodyAsync(Stream stream);
}