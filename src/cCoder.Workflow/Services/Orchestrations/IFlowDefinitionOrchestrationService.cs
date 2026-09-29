// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Workflow.Models.Results;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

internal interface IFlowDefinitionOrchestrationService
{
    FlowDefinition Get(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition);

    ValueTask DeleteAsync(Guid flowDefinitionId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdateFlowDefinition(IEnumerable<FlowDefinition> items);

    ValueTask DeleteAllFlowDefinitionAsync(IEnumerable<FlowDefinition> deletedItems);
}