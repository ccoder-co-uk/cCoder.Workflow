// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal interface IFlowDefinitionService
{
    FlowDefinition Get(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition);

    ValueTask DeleteAsync(Guid flowDefinitionId);

    ValueTask DeleteWithInstancesAsync(Guid flowDefinitionId);

    ValueTask DeleteWithInstancesByAppIdAsync(int appId);

    bool AuthorizeExecution(string userId, int? appId);
    object ParseDefinition(string definitionJson);
    object ParseData(string args);
    string SerializeContext(object context);
    bool LogFlowDefinitionAddOrUpdate(IEnumerable<FlowDefinition> flowDefinitions);
}