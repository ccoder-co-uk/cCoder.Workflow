// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Coordinations;

internal interface IFlowDefinitionCoordinationService
{
    ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition);

    ValueTask<Guid> QueueAsync(Guid flowDefinitionId, string asUserId, string args);
}