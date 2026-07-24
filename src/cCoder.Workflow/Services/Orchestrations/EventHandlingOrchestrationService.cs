// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Dependencies;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class EventHandlingOrchestrationService(
    IWorkflowEventProcessingService workflowEventProcessingService,
    IFlowDefinitionCoordinationService flowDefinitionCoordinationService,
    IJsonBroker jsonBroker,
    ILogger<EventHandlingOrchestrationService> log)
    : IEventHandlingOrchestrationService
{
    public Task RaiseEvents(object payload, string eventName, int? appIdOverride = null) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [payload, eventName, appIdOverride]); await ExecuteRaiseEvents(payload: payload, eventName: eventName, appIdOverride: appIdOverride); });

    private async Task ExecuteRaiseEvents(object payload, string eventName, int? appIdOverride = null)
    {
        int? appId = appIdOverride ?? GetIntProperty(payload: payload, propertyName: "AppId");

        if (appId == null)
        {
            return;
        }

        string context = GetStringProperty(payload: payload, propertyName: "Path") ?? string.Empty;
        string eventContext = $"{eventName}{context}";

        log.LogDebug(
            message: "Workflow trigger event: AppId {AppId}, Context {EventContext}",
            args: [appId, eventContext]);

        WorkflowEvent[] subscriptions = await workflowEventProcessingService.GetSubscriptionsAsync(
appId: appId.Value,
eventContext: eventContext);

        if (subscriptions.Length == 0)
        {
            return;
        }

        log.LogDebug(message: "Found {Count} subscribers, calling ...", args: subscriptions.Length);

        string args = jsonBroker.Serialize(value: payload);
        IEnumerable<Task> workload = subscriptions.Select(selector: subscription => QueueHandlingFlowInstanceSafelyAsync(subscription: subscription, args: args));
        await Task.WhenAll(tasks: workload);
    }

    private async Task QueueHandlingFlowInstanceSafelyAsync(WorkflowEvent subscription, string args)
    {
        try
        {
            _ = await flowDefinitionCoordinationService.QueueAsync(
flowDefinitionId: subscription.FlowId,
asUserId: subscription.ExecuteAsUser?.Id ?? subscription.ExecuteAs,
args: args);
        }
        catch (Exception ex)
        {
            log.LogWarning(
                exception: ex,
                message: "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
                args: [subscription.Id, subscription.FlowId]);
        }
    }

    private static int? GetIntProperty(object payload, string propertyName) =>
        payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload) as int?
        ?? (payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload) is int value ? value : null);

    private static string GetStringProperty(object payload, string propertyName) =>
        payload.GetType()
            .GetProperty(name: propertyName)?.GetValue(obj: payload)?.ToString();
}