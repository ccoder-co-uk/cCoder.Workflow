// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Models;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowInstanceService(
    IWorkflowInstanceManagementBroker workflowInstanceManagementBroker,
    IFlowInstanceDataManager flowInstanceDataManager,
    ITokenManager tokenManager,
    IWorkflowExecutionEventBroker workflowExecutionEventBroker,
    WorkflowConfiguration workflowConfiguration,
    ILoggingBroker loggingBroker)
    : IWorkflowInstanceService
{
    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(inputs: [ignoreFilters]);
            return flowInstanceDataManager.GetAll(ignoreFilters: ignoreFilters);
        });

    public object[] GetFailedExecutionStats() =>
        TryCatch(operation: () =>
        {
            return workflowInstanceManagementBroker.GetFailedExecutionStats();
        });

    public bool IsMigrating() =>
        TryCatch(operation: () =>
        {
            return workflowConfiguration.IsMigrating;
        });

    public FlowInstanceData[] GetQueuedFlowInstanceData() =>
        TryCatch(operation: () =>
        {
            return workflowInstanceManagementBroker.GetQueuedInstances();
        });

    public ValueTask<int> DeleteOldFlowInstanceDataAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken) =>
        TryCatch(
            operation: async () =>
            {
                ValidateOldFlowInstanceDataOnDelete(
                    inputs: [cutoff, cancellationToken]);

                return await workflowInstanceManagementBroker.FlushOldInstancesAsync(
                    cutoff: cutoff,
                    cancellationToken: cancellationToken);
            },
            isValueTask: true);

    public ValueTask<FlowInstanceData> GetClaimedFlowInstanceDataAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken) =>
        TryCatch(
            operation: async () =>
            {
                ValidateClaimedFlowInstanceDataOnGet(
                    inputs: [flowInstanceDataId, cancellationToken]);

                return await workflowInstanceManagementBroker.SelectClaimedInstanceAsync(
                    flowInstanceDataId: flowInstanceDataId,
                    cancellationToken: cancellationToken);
            },
            isValueTask: true);

    public ValueTask<int> ClaimQueuedFlowInstanceDataAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [flowInstanceDataId, cancellationToken]);

                return await workflowInstanceManagementBroker
                    .UpdateQueuedInstanceClaimAsync(
                        flowInstanceDataId: flowInstanceDataId,
                        cancellationToken: cancellationToken);
            },
            isValueTask: true);

    public ValueTask RaiseFlowInstanceDataWorkflowExecutionAsync(
        FlowInstanceData flowInstanceData) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [flowInstanceData]);

                Token token = await tokenManager.IssueTokenAsync(
                    userId: flowInstanceData.Caller,
                    tokenUse: TokenUse.WorkflowExecution);

                WorkflowRequest request = new()
                {
                    Api = CreateApiRoot(domain: flowInstanceData.FlowDefinition.App.Domain),
                    FlowId = flowInstanceData.FlowDefinition.Id,
                    AuthToken = token.Id,
                    InstanceId = flowInstanceData.Id
                };

                await workflowExecutionEventBroker.RaiseWorkflowExecuteEventAsync(
                    message: new EventMessage<WorkflowRequest>
                    {
                        AuthInfo = new EventAuthInfo { SSOUserId = flowInstanceData.Caller },
                        Data = request
                    });
            },
            isValueTask: true);

    public bool LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [exception]);
            loggingBroker.LogError(exception: exception, message: exception.Message);

            if (exception.InnerException is not null)
            {
                loggingBroker.LogError(exception: exception.InnerException, message: exception.InnerException.Message);
            }

            return true;
        });

    public bool LogDroppedFlowInstanceData(int count, TimeSpan maxAge) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [count, maxAge]);
            loggingBroker.LogInformation(message: "Dropped {Count} Workflow instances older than {MaxAge}.", args: [count, maxAge]);
            return true;
        });

    public bool LogFlowInstanceDataExecutionFailure(Guid flowInstanceDataId, Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [flowInstanceDataId, exception]);
            loggingBroker.LogError(exception: exception, message: "Flow instance {InstanceId} execution failed.", args: flowInstanceDataId);
            return true;
        });

    public int GetSslPort() =>
        TryCatch(operation: () => workflowConfiguration.SslPort);

    public TimeSpan GetInstanceMaintenanceMaxAge() =>
        TryCatch(operation: () => TimeSpan.FromDays(value: workflowConfiguration.InstanceMaintenance.MaxAgeDays));

    public TimeSpan GetExecutingInstanceTimeout() =>
        TryCatch(operation: () => TimeSpan.FromMinutes(value: workflowConfiguration.QueueInstanceManagement.ExecutingTimeoutMinutes));

    public TimeSpan GetQueuePollingInterval() =>
        TryCatch(operation: () => TimeSpan.FromMilliseconds(milliseconds: workflowConfiguration.QueueInstanceManagement.PollingIntervalMilliseconds));

    private string CreateApiRoot(string domain)
    {
        int sslPort = workflowConfiguration.SslPort;
        string port = sslPort is > 0 and not 443 ? $":{sslPort}" : string.Empty;
        return $"https://{domain}{port}/Api/";
    }
}
