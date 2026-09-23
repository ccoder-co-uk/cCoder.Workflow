// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Security.Models.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class WorkflowInstanceAggregationService(
    IWorkflowInstanceManagementService workflowInstanceManagementService,
    IFlowInstanceDataService flowInstanceDataService,
    IWorkflowTokenService workflowTokenService,
    IWorkflowExecutionEventService workflowExecutionEventService,
    IWorkflowConfigurationService workflowConfigurationService,
    ILoggingBroker loggingBroker)
    : IWorkflowInstanceAggregationService
{
    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(inputs: [ignoreFilters]);

            return flowInstanceDataService.GetAll(ignoreFilters: ignoreFilters);
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
            LogError(exception: ex.InnerException ?? ex);
        }
    }

    public object[] GetStats() =>
        TryCatch(operation: () => { return ExecuteGetStats(); });

    private object[] ExecuteGetStats()
            =>
        workflowInstanceManagementService.GetFailedExecutionStats();

    public Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunInstanceMaintenanceContinuouslyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfigurationService.IsMigrating())
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
            LogError(exception: ex.InnerException ?? ex);
        }
    }

    public Task RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [cancellationToken]); await ExecuteRunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(cancellationToken: cancellationToken); });

    private async Task ExecuteRunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfigurationService.IsMigrating())
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
            LogError(exception: ex.InnerException ?? ex);
        }
    }

    private async ValueTask ExecuteQueuedInstancesAsync(CancellationToken cancellationToken)
    {
        FlowInstanceData[] queuedInstances =
            workflowInstanceManagementService.GetQueuedFlowInstanceData();

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
        int dropCount = await workflowInstanceManagementService
            .DeleteOldFlowInstanceDataAsync(
                cutoff: DateTimeOffset.UtcNow.Subtract(
                    value: GetInstanceMaintenanceMaxAge()),
                cancellationToken: cancellationToken);

        if (dropCount > 0)
        {
            loggingBroker.LogInformation(
                message: "Dropped {Count} Workflow instances older than {MaxAge}.",
                args: [dropCount, GetInstanceMaintenanceMaxAge()]);
        }
    }

    private async Task ExecuteInstanceAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        int claimedCount = await workflowInstanceManagementService
            .ClaimQueuedFlowInstanceDataAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: cancellationToken);

        if (claimedCount == 0)
        {
            return;
        }

        FlowInstanceData dbInstance = await workflowInstanceManagementService
            .GetClaimedFlowInstanceDataAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: cancellationToken);

        if (dbInstance is null || dbInstance.State != "AwaitingExecution")
        {
            return;
        }

        try
        {
            Token token = await workflowTokenService.IssueWorkflowExecutionTokenAsync(
                userId: dbInstance.Caller);

            WorkflowRequest request = CreateWorkflowRequest(
                dbInstance: dbInstance,
                token: token);

            await workflowExecutionEventService.RaiseWorkflowRequestEventMessageAsync(
                message: new EventMessage<WorkflowRequest>
                {
                    AuthInfo = new EventAuthInfo { SSOUserId = dbInstance.Caller },
                    Data = request
                });
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception.InnerException ?? exception,
                message: "Flow instance {InstanceId} execution failed.",
                args: dbInstance.Id);
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
        int sslPort = workflowConfigurationService.GetSslPort();
        string port = sslPort is > 0 and not 443 ? $":{sslPort}" : string.Empty;

        return $"https://{domain}{port}/Api/";
    }

    private TimeSpan GetInstanceMaintenanceMaxAge() =>
        workflowConfigurationService.GetInstanceMaintenanceMaxAge();

    private TimeSpan GetExecutingInstanceTimeout() =>
        workflowConfigurationService.GetExecutingInstanceTimeout();

    private TimeSpan GetQueuePollingInterval() =>
        workflowConfigurationService.GetQueuePollingInterval();

    private void LogError(Exception exception)
    {
        loggingBroker.LogError(exception: exception, message: exception.Message);

        if (exception.InnerException is not null)
        {
            loggingBroker.LogError(
                exception: exception.InnerException,
                message: exception.InnerException.Message);
        }
    }
}