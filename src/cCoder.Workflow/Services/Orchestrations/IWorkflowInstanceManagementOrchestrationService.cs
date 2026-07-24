// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

public interface IWorkflowInstanceManagementOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);

    Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default);

    Task RunInstanceMaintenanceAsync(CancellationToken cancellationToken = default);

    Task RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default);

    Task RunQueueInstanceBackgroundServiceDependencyAsync(CancellationToken cancellationToken = default);

    object[] GetStats();

    ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid flowInstanceDataId);
}