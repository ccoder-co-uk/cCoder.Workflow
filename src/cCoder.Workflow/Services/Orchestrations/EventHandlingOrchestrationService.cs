using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed class EventHandlingOrchestrationService(
    IWorkflowEventProcessingService workflowEventProcessingService,
    IFlowInstanceDataProcessingService flowInstanceDataProcessingService,
    IFlowInstanceDataEventProcessingService flowInstanceDataEventProcessingService,
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
            FlowInstanceData flowInstance = CreateFlowInstanceData(
                subscription.Flow,
                subscription.ExecuteAsUser?.Id ?? subscription.ExecuteAs,
                args);

            FlowInstanceData savedFlowInstance = await flowInstanceDataProcessingService.AddQueuedAsync(flowInstance);
            await flowInstanceDataEventProcessingService.RaiseFlowInstanceDataAddEventAsync(savedFlowInstance);
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

    private FlowInstanceData CreateFlowInstanceData(FlowDefinition flowDefinition, string caller, string args)
    {
        Guid instanceId = Guid.NewGuid();
        Flow flow = ParseFlow(flowDefinition?.DefinitionJson);

        if (flow == null)
            throw new InvalidOperationException("Flow definition does not contain a valid workflow definition.");

        WorkflowContext context = new()
        {
            ExecutionState = "Queued",
            InstanceId = instanceId,
            Flow = flow,
            Variables = new Dictionary<string, object> { { "Data", args } },
            ExecutionLog = Array.Empty<WorkflowLogEntry>()
        };

        Start start = context.Flow.Activities.OfType<Start>().FirstOrDefault();

        if (start == null)
            throw new InvalidOperationException("Flow definition does not contain a Start activity.");

        start.Data = jsonBroker.ParseJson(args);

        return new FlowInstanceData
        {
            Id = instanceId,
            State = "Queued",
            FlowDefinitionId = flowDefinition.Id,
            Start = DateTimeOffset.UtcNow,
            Caller = caller,
            ContextString = jsonBroker.Serialize(context),
            FlowDefinition = flowDefinition
        };
    }

    private Flow ParseFlow(string definitionJson) =>
        string.IsNullOrWhiteSpace(definitionJson)
            ? null
            : jsonBroker.ParseJson<Flow>(definitionJson);

    private static int? GetIntProperty(object payload, string propertyName) =>
        payload.GetType().GetProperty(propertyName)?.GetValue(payload) as int?
        ?? (payload.GetType().GetProperty(propertyName)?.GetValue(payload) is int value ? value : null);

    private static string GetStringProperty(object payload, string propertyName) =>
        payload.GetType().GetProperty(propertyName)?.GetValue(payload)?.ToString();
}
