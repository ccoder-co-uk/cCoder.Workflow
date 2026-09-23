// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowInstanceManagementService(
    IWorkflowInstanceManagementBroker workflowInstanceManagementBroker)
    : IWorkflowInstanceManagementService
{
    public object[] GetFailedExecutionStats() =>
        TryCatch(operation: () => workflowInstanceManagementBroker.GetFailedExecutionStats());

    public FlowInstanceData[] GetQueuedFlowInstanceData() =>
        TryCatch(operation: () => workflowInstanceManagementBroker.GetQueuedInstances());

    public ValueTask<int> DeleteOldFlowInstanceDataAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken) =>
        TryCatch(
            operation: async () =>
            {
                ValidateOldFlowInstanceDataOnDelete(inputs: [cutoff, cancellationToken]);

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

                return await workflowInstanceManagementBroker.UpdateQueuedInstanceClaimAsync(
                    flowInstanceDataId: flowInstanceDataId,
                    cancellationToken: cancellationToken);
            },
            isValueTask: true);
}