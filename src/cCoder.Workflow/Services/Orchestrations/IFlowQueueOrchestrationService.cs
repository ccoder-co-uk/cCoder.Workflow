// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace cCoder.Workflow.Services.Orchestrations;

internal interface IFlowQueueOrchestrationService
{
    ValueTask<Guid> QueueFlowDefinitionAsync(
        Guid flowDefinitionId,
        string asUserId,
        string args);
}