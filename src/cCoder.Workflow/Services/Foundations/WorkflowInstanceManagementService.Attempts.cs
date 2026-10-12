// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Models;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowInstanceManagementService
{
    public ValueTask<FlowInstanceData> PrepareQueuedInstanceAsync(Guid instanceId, CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [instanceId, cancellationToken]);

            FlowInstanceData expected = await workflowInstanceManagementBroker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId, cancellationToken: cancellationToken);

            if (expected is null || expected.State != "Queued")
            {
                return null;
            }


            FlowInstanceData replacement = CopyAttemptInstance(instance: expected);
            WorkflowAttemptContext context;

            try
            {
                context = jsonBroker.ParseAttemptContext(json: expected.ContextString)
                    ?? throw new InvalidOperationException(message: "Workflow context is empty.");
            }
            catch (Exception)
            {
                replacement.State = "Failed";
                replacement.End = DateTimeOffset.UtcNow;

                _ = await workflowInstanceManagementBroker.CompareExchangeInstanceAsync(
                    workflowInstanceExchange: new cCoder.Workflow.Models.WorkflowInstanceExchange { Expected = expected, Replacement = replacement }, cancellationToken: cancellationToken);

                throw new InvalidOperationException(message: "Workflow context could not be read; original context preserved.");
            }

            context.ExecutionAttemptId = Guid.NewGuid();
            context.ExecutionOwnerId = null;
            replacement.ContextString = jsonBroker.SerializeAttemptContext(workflowAttemptContext: context);
            replacement.State = "AwaitingExecution";
            replacement.Start = DateTimeOffset.UtcNow;
            replacement.End = null;

            int updated = await workflowInstanceManagementBroker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: new cCoder.Workflow.Models.WorkflowInstanceExchange { Expected = expected, Replacement = replacement }, cancellationToken: cancellationToken);

            return updated == 1 ? replacement : null;
        }, isValueTask: true);

    public ValueTask<bool> RecordTerminalFailureAsync(
        Guid instanceId, Guid flowId, Guid attemptId, string failureDescription,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [instanceId, flowId, attemptId, failureDescription, cancellationToken]);

            if (attemptId == Guid.Empty || string.IsNullOrWhiteSpace(value: failureDescription))
            {
                return false;
            }

            FlowInstanceData expected = await workflowInstanceManagementBroker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId, cancellationToken: cancellationToken);

            if (expected is null || expected.FlowDefinitionId != flowId
                || expected.State != "AwaitingExecution")
            {
                return false;
            }

            WorkflowAttemptContext context = jsonBroker.ParseAttemptContext(json: expected.ContextString);

            if (context?.ExecutionAttemptId != attemptId)
            {
                return false;
            }

            context.ExecutionState = "Failed";
            context.ExecutionLog ??= [];

            context.ExecutionLog.Add(item: jsonBroker.SerializeElement(value: new WorkflowLogEntry(
                level: WorkflowLogLevel.Error, message: failureDescription)));

            FlowInstanceData replacement = CopyAttemptInstance(instance: expected);
            replacement.ContextString = jsonBroker.SerializeAttemptContext(workflowAttemptContext: context);
            replacement.State = "Failed";
            replacement.End = DateTimeOffset.UtcNow;

            return await workflowInstanceManagementBroker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: new cCoder.Workflow.Models.WorkflowInstanceExchange { Expected = expected, Replacement = replacement }, cancellationToken: cancellationToken) == 1;
        }, isValueTask: true);

    public FlowInstanceData[] GetExpiredFlowInstanceData(DateTimeOffset cutoff) =>
        TryCatch(operation: () =>
        {
            ValidateExpiredFlowInstanceDataOnGet(inputs: [cutoff]);

            return workflowInstanceManagementBroker.SelectExpiredInstances(cutoff: cutoff);
        });

    public ValueTask<int> FailExpiredFlowInstanceDataAsync(FlowInstanceData flowInstanceData, DateTimeOffset cutoff, CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowInstanceData, cutoff, cancellationToken]);

            if (flowInstanceData is null || flowInstanceData.Start > cutoff
                || flowInstanceData.State is not ("AwaitingExecution" or "Executing"))
            {
                return 0;
            }

            FlowInstanceData replacement = CopyAttemptInstance(instance: flowInstanceData);

            try
            {
                WorkflowAttemptContext context = jsonBroker.ParseAttemptContext(json: flowInstanceData.ContextString)
                    ?? throw new InvalidOperationException(message: "Workflow context is empty.");

                context.ExecutionState = "Failed";
                context.ExecutionLog ??= [];

                context.ExecutionLog.Add(item: jsonBroker.SerializeElement(value: new WorkflowLogEntry(
                    level: WorkflowLogLevel.Error,
                    message: "Workflow execution did not finish within its configured timeout. Marked Failed for administrative recovery; it was not rerun.")));

                replacement.ContextString = jsonBroker.SerializeAttemptContext(workflowAttemptContext: context);
            }
            catch (Exception)
            {
                replacement.ContextJson = flowInstanceData.ContextJson;
            }

            replacement.State = "Failed";
            replacement.End = DateTimeOffset.UtcNow;

            return await workflowInstanceManagementBroker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: new WorkflowInstanceExchange { Expected = flowInstanceData, Replacement = replacement },
                cancellationToken: cancellationToken);
        }, isValueTask: true);

    private static FlowInstanceData CopyAttemptInstance(FlowInstanceData instance) =>
        new()
    {
        Id = instance.Id,
        FlowDefinitionId = instance.FlowDefinitionId,
        FlowDefinition = instance.FlowDefinition,
        Caller = instance.Caller,
        ContextJson = instance.ContextJson,
        State = instance.State,
        Start = instance.Start,
        End = instance.End,
        Name = instance.Name,
        ReportingComponentName = instance.ReportingComponentName
    };
}