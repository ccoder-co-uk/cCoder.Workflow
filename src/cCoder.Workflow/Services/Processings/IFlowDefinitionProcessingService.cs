// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IFlowDefinitionProcessingService
{
    FlowDefinition Get(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newEntity);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedEntity);

    ValueTask DeleteAsync(Guid flowDefinitionId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdateFlowDefinition(IEnumerable<FlowDefinition> items);

    ValueTask DeleteAllFlowDefinitionAsync(IEnumerable<FlowDefinition> deletedItems);
}