// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface IFlowDefinitionService
{
    FlowDefinition Get(Guid flowDefinitionId);
    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);
    ValueTask<FlowDefinition> AddAsync(FlowDefinition flowDefinition);
    ValueTask<FlowDefinition> UpdateAsync(FlowDefinition flowDefinition);
    ValueTask DeleteAsync(Guid flowDefinitionId);
    ValueTask DeleteWithInstancesAsync(Guid flowDefinitionId);
    ValueTask DeleteWithInstancesByAppIdAsync(int appId);
}