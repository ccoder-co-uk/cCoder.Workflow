// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures;

public interface IFlowDefinitionManager
{
    FlowDefinition GetFlowDefinition(Guid flowDefinitionId);

    IQueryable<FlowDefinition> GetAllFlowDefinitions();

    ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition);

    ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition);

    ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId);

    ValueTask<Guid> QueueFlowDefinitionAsync(Guid flowDefinitionId, string asUserId, string args);

    ValueTask<string> ExecuteScriptAsync(string script);

    ValueTask<string> ReadRequestBodyAsync(Stream stream);

}