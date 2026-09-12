// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowDefinitionBroker
{
    IQueryable<FlowDefinition> SelectAllFlowDefinitions();

    IQueryable<FlowDefinition> SelectAllFlowDefinitionsIgnoringQueryFilters();

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition);

    ValueTask<int> DeleteFlowDefinitionAsync(FlowDefinition deletedFlowDefinition);

    ValueTask DeleteFlowDefinitionWithInstancesAsync(Guid flowDefinitionId);

    ValueTask DeleteFlowDefinitionsWithInstancesByAppIdAsync(int appId);

    int? SelectAppId(FlowDefinition flowDefinition);
}