// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class WorkflowEventCoordinationService(
    IWorkflowEventOrchestrationService workflowEventOrchestrationService,
    IFlowQueueOrchestrationService flowQueueOrchestrationService)
    : IWorkflowEventCoordinationService
{
    public Task RaiseEvents(object payload, string eventName, int? appIdOverride = null) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [payload, eventName, appIdOverride]); await ExecuteRaiseEvents(payload: payload, eventName: eventName, appIdOverride: appIdOverride); });

    private async Task ExecuteRaiseEvents(object payload, string eventName, int? appIdOverride = null)
    {
        (int? appId, string eventContext) =
            workflowEventOrchestrationService.PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: eventName,
                appIdOverride: appIdOverride);

        if (appId == null)
        {
            return;
        }

        WorkflowEvent[] subscriptions =
            await workflowEventOrchestrationService.GetSubscribedWorkflowEventsAsync(
                appId: appId.Value,
                eventContext: eventContext);

        if (subscriptions.Length == 0)
        {
            return;
        }

        string args = workflowEventOrchestrationService.SerializeWorkflowEventPayload(payload: payload);

        IEnumerable<Task> workload = subscriptions.Select(
            selector: subscription =>
                QueueHandlingFlowInstanceSafelyAsync(
                    subscription: subscription,
                    args: args));

        await Task.WhenAll(tasks: workload);
    }

    private async Task QueueHandlingFlowInstanceSafelyAsync(WorkflowEvent subscription, string args)
    {
        try
        {
            _ = await flowQueueOrchestrationService.QueueFlowDefinitionAsync(
                flowDefinitionId: subscription.FlowId,
                asUserId: subscription.ExecuteAsUser?.Id ?? subscription.ExecuteAs,
                args: args);
        }
        catch (Exception ex)
        {
            await workflowEventOrchestrationService.LogWorkflowEventQueueFailureAsync(
                workflowEvent: subscription,
                exception: ex);
        }
    }
}