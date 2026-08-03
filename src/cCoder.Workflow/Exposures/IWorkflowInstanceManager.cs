// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures;

public interface IWorkflowInstanceManager
{
    Task RunAsync(CancellationToken cancellationToken = default);
    Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default);
    Task RunInstanceMaintenanceAsync(CancellationToken cancellationToken = default);
    Task RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default);
    Task RunQueueInstanceBackgroundServiceDependencyAsync(CancellationToken cancellationToken = default);
    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);
    object[] GetStats();
    ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid flowInstanceDataId);
}
