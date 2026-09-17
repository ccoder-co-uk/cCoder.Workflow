// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Security.Models.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowInstanceProcessingService(
    IWorkflowInstanceService workflowInstanceService)
    : IWorkflowInstanceProcessingService
{
    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return workflowInstanceService.GetAll(ignoreFilters: ignoreFilters);
        });

    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);
            await RunQueueInstanceBackgroundServiceDependencyAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _ = workflowInstanceService.LogError(
                exception: ex.InnerException ?? ex);
        }
    }

    public object[] GetStats() =>
        TryCatch(operation: () => { return ExecuteGetStats(); });

    private object[] ExecuteGetStats()
            =>
        workflowInstanceService.GetFailedExecutionStats();

    public Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunInstanceMaintenanceContinuouslyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowInstanceService.IsMigrating())
        {
            return;
        }

        await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);
        }
    }

    public Task RunInstanceMaintenanceAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunInstanceMaintenanceAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunInstanceMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DropOldInstancesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _ = workflowInstanceService.LogError(
                exception: ex.InnerException ?? ex);
        }
    }

    public Task RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowInstanceService.IsMigrating())
        {
            return;
        }

        await RunQueueInstanceBackgroundServiceDependencyAsync(cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(GetQueuePollingInterval());

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await RunQueueInstanceBackgroundServiceDependencyAsync(cancellationToken: cancellationToken);
        }
    }

    public Task RunQueueInstanceBackgroundServiceDependencyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunQueueInstanceBackgroundServiceDependencyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunQueueInstanceBackgroundServiceDependencyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await ExecuteQueuedInstancesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _ = workflowInstanceService.LogError(
                exception: ex.InnerException ?? ex);
        }
    }

    private async ValueTask ExecuteQueuedInstancesAsync(CancellationToken cancellationToken)
    {
        FlowInstanceData[] queuedInstances =
            workflowInstanceService.GetQueuedFlowInstanceData();

        foreach (FlowInstanceData queuedInstance in queuedInstances)
        {
            await ExecuteInstanceAsync(instanceId: queuedInstance.Id, cancellationToken: cancellationToken);
        }
    }

    public ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private async ValueTask ExecuteExecuteWaitingQueuedInstanceByIdAsync(Guid flowInstanceDataId)
    {
        await ExecuteInstanceAsync(instanceId: flowInstanceDataId);
    }

    private async ValueTask DropOldInstancesAsync(CancellationToken cancellationToken)
    {
        int dropCount = await workflowInstanceService
            .DeleteOldFlowInstanceDataAsync(
                cutoff: DateTimeOffset.UtcNow.Subtract(
                    value: GetInstanceMaintenanceMaxAge()),
                cancellationToken: cancellationToken);

        if (dropCount > 0)
        {
            _ = workflowInstanceService.LogDroppedFlowInstanceData(
                count: dropCount,
                maxAge: GetInstanceMaintenanceMaxAge());
        }
    }

    private async Task ExecuteInstanceAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        int claimedCount = await workflowInstanceService
            .ClaimQueuedFlowInstanceDataAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: cancellationToken);

        if (claimedCount == 0)
        {
            return;
        }

        FlowInstanceData dbInstance = await workflowInstanceService
            .GetClaimedFlowInstanceDataAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: cancellationToken);

        if (dbInstance is null || dbInstance.State != "AwaitingExecution")
        {
            return;
        }

        try
        {
            await workflowInstanceService.RaiseFlowInstanceDataWorkflowExecutionAsync(
                flowInstanceData: dbInstance);
        }
        catch (Exception exception)
        {
            _ = workflowInstanceService.LogFlowInstanceDataExecutionFailure(
                flowInstanceDataId: dbInstance.Id,
                exception: exception.InnerException ?? exception);
        }
    }

    internal WorkflowRequest CreateWorkflowRequest(FlowInstanceData dbInstance, Token token) =>
        new()
        {
            Api = CreateApiRoot(domain: dbInstance.FlowDefinition.App.Domain),
            FlowId = dbInstance.FlowDefinition.Id,
            AuthToken = token.Id,
            InstanceId = dbInstance.Id
        };

    private string CreateApiRoot(string domain)
    {
        int sslPort = workflowInstanceService.GetSslPort();
        string port = sslPort is > 0 and not 443 ? $":{sslPort}" : string.Empty;

        return $"https://{domain}{port}/Api/";
    }

    private TimeSpan GetInstanceMaintenanceMaxAge() =>
        workflowInstanceService.GetInstanceMaintenanceMaxAge();

    private TimeSpan GetExecutingInstanceTimeout() =>
        workflowInstanceService.GetExecutingInstanceTimeout();

    private TimeSpan GetQueuePollingInterval() =>
        workflowInstanceService.GetQueuePollingInterval();
}
