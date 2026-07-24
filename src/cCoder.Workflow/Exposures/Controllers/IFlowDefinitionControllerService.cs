// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures.Controllers;

public interface IFlowDefinitionControllerService
{
    FlowDefinition Get(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAll();

    ValueTask<FlowDefinition> PostFlowDefinitionAsync(FlowDefinition newEntity);

    ValueTask<FlowDefinition> PutFlowDefinitionAsync(FlowDefinition updatedEntity);

    ValueTask DeleteAsync(Guid flowDefinitionId);

    ValueTask<Guid> PostFlowDefinitionQueueAsync(Guid flowDefinitionId, string asUserId, string args);

    Task<string> PostScriptAsync(string script);

    MetadataContainerSet[] GetKnownActivityTypes();

    MetadataContainerSet[] GetKnownSystemTypes();
}