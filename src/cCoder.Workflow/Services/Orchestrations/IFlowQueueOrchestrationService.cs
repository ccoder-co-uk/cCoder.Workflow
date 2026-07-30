// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

internal interface IFlowQueueOrchestrationService
{
    ValueTask<Guid> QueueFlowDefinitionAsync(
        Guid flowDefinitionId,
        string asUserId,
        string args);
}