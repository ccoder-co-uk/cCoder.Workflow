// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowInstanceManagementService
{
    ValueTask<bool> RecordTerminalFailureAsync(
        Guid instanceId,
        Guid flowId,
        Guid attemptId,
        string failureDescription,
        CancellationToken cancellationToken = default);

    ValueTask<FlowInstanceData> PrepareQueuedInstanceAsync(Guid instanceId, CancellationToken cancellationToken);

    FlowInstanceData[] GetExpiredFlowInstanceData(DateTimeOffset cutoff);

    ValueTask<int> FailExpiredFlowInstanceDataAsync(FlowInstanceData flowInstanceData, DateTimeOffset cutoff, CancellationToken cancellationToken);

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