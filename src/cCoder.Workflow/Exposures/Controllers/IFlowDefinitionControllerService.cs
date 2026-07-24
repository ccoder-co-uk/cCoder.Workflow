// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures.Controllers;

public interface IFlowDefinitionControllerService
{
    FlowDefinition Get(Guid id);

    IQueryable<FlowDefinition> GetAll();

    ValueTask<FlowDefinition> AddAsync(FlowDefinition entity);

    ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<Guid> QueueAsync(Guid id, string asUserId, string args);

    Task<string> ExecuteScriptAsync(string script);

    MetadataContainerSet[] GetKnownActivityTypes();

    MetadataContainerSet[] GetKnownSystemTypes();
}