// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed class EventHandlingOrchestrationService(
    IWorkflowEventProcessingService workflowEventProcessingService,
    IFlowDefinitionCoordinationService flowDefinitionCoordinationService,
    IJsonBroker jsonBroker,
    ILogger<EventHandlingOrchestrationService> log)
    : IEventHandlingOrchestrationService
{
    public async Task RaiseEvents(object payload, string eventName, int? appIdOverride = null)
    {
        int? appId = appIdOverride ?? GetIntProperty(payload, "AppId");

        if (appId == null)
            return;

        string context = GetStringProperty(payload, "Path") ?? string.Empty;
        string eventContext = $"{eventName}{context}";

        log.LogDebug("Workflow trigger event: AppId {AppId}, Context {EventContext}", appId, eventContext);

        WorkflowEvent[] subscriptions = await workflowEventProcessingService.GetSubscriptionsAsync(
            appId.Value,
            eventContext);

        if (subscriptions.Length == 0)
            return;

        log.LogDebug("Found {Count} subscribers, calling ...", subscriptions.Length);

        string args = jsonBroker.Serialize(payload);
        IEnumerable<Task> workload = subscriptions.Select(subscription => QueueHandlingFlowInstanceSafelyAsync(subscription, args));
        await Task.WhenAll(workload);
    }

    private async Task QueueHandlingFlowInstanceSafelyAsync(WorkflowEvent subscription, string args)
    {
        try
        {
            _ = await flowDefinitionCoordinationService.QueueAsync(
                subscription.FlowId,
                subscription.ExecuteAsUser?.Id ?? subscription.ExecuteAs,
                args);
        }
        catch (Exception ex)
        {
            log.LogWarning(
                ex,
                "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
                subscription.Id,
                subscription.FlowId);
        }
    }

    private static int? GetIntProperty(object payload, string propertyName) =>
        payload.GetType().GetProperty(propertyName)?.GetValue(payload) as int?
        ?? (payload.GetType().GetProperty(propertyName)?.GetValue(payload) is int value ? value : null);

    private static string GetStringProperty(object payload, string propertyName) =>
        payload.GetType().GetProperty(propertyName)?.GetValue(payload)?.ToString();
}