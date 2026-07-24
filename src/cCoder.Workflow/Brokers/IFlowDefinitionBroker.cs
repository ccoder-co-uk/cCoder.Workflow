// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowDefinitionBroker
{
    IQueryable<FlowDefinition> GetAllFlowDefinitions(bool ignoreFilters);
    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition entity);
    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition entity);
    ValueTask<int> DeleteFlowDefinitionAsync(FlowDefinition entity);
    ValueTask DeleteFlowDefinitionWithInstancesAsync(Guid id);
    ValueTask DeleteFlowDefinitionsWithInstancesByAppIdAsync(int appId);
    ValueTask DeleteAllFlowDefinitionsAsync(IEnumerable<FlowDefinition> items);
    int? GetAppId(FlowDefinition entity);
}