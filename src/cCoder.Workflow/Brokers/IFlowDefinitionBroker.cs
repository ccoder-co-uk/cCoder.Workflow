// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowDefinitionBroker
{
    IQueryable<FlowDefinition> SelectAllFlowDefinitions();

    IQueryable<FlowDefinition> SelectAllFlowDefinitionsIgnoringQueryFilters();

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newEntity);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedEntity);

    ValueTask<int> DeleteFlowDefinitionAsync(FlowDefinition deletedEntity);

    ValueTask DeleteFlowDefinitionWithInstancesAsync(Guid flowDefinitionId);

    ValueTask DeleteFlowDefinitionsWithInstancesByAppIdAsync(int appId);

    int? SelectAppId(FlowDefinition entity);
}