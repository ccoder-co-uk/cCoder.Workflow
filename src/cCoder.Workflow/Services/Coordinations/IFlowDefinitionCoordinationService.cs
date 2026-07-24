// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Coordinations;

public interface IFlowDefinitionCoordinationService
{
    ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition);

    ValueTask<Guid> QueueAsync(Guid flowDefinitionId, string asUserId, string args);
}