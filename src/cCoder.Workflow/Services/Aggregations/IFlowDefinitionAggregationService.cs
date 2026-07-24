// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Aggregations;

public interface IFlowDefinitionAggregationService
{
    FlowDefinition GetFlowDefinition(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAllFlowDefinitions();

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newEntity);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedEntity);

    ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId);

    ValueTask<Guid> QueueFlowDefinitionAsync(Guid flowDefinitionId, string asUserId, string args);

    ValueTask<string> ExecuteScriptAsync(string script);

    MetadataContainerSet[] GetKnownActivityTypes();

    MetadataContainerSet[] GetKnownSystemTypes();
}