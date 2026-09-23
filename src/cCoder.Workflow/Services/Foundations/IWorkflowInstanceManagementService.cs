// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowInstanceManagementService
{
    object[] GetFailedExecutionStats();

    FlowInstanceData[] GetQueuedFlowInstanceData();

    ValueTask<int> DeleteOldFlowInstanceDataAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken);

    ValueTask<int> ClaimQueuedFlowInstanceDataAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken);

    ValueTask<FlowInstanceData> GetClaimedFlowInstanceDataAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken);
}