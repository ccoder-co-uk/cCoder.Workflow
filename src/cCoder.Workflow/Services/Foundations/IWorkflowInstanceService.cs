// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowInstanceService
{
    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);
    object[] GetFailedExecutionStats();
    bool IsMigrating();
    FlowInstanceData[] GetQueuedFlowInstanceData();
    ValueTask<int> DeleteOldFlowInstanceDataAsync(DateTimeOffset cutoff, CancellationToken cancellationToken);
    ValueTask<int> ClaimQueuedFlowInstanceDataAsync(Guid flowInstanceDataId, CancellationToken cancellationToken);
    ValueTask<FlowInstanceData> GetClaimedFlowInstanceDataAsync(Guid flowInstanceDataId, CancellationToken cancellationToken);
    ValueTask RaiseFlowInstanceDataWorkflowExecutionAsync(FlowInstanceData flowInstanceData);
    bool LogError(Exception exception);
    bool LogDroppedFlowInstanceData(int count, TimeSpan maxAge);
    bool LogFlowInstanceDataExecutionFailure(Guid flowInstanceDataId, Exception exception);
    int GetSslPort();
    TimeSpan GetInstanceMaintenanceMaxAge();
    TimeSpan GetExecutingInstanceTimeout();
    TimeSpan GetQueuePollingInterval();
}
