using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Orchestrations;

internal static class FlowInstanceDataFactory
{
    public static FlowInstanceData Create(
        FlowDefinition flowDefinition,
        string caller,
        string args,
        IJsonBroker jsonBroker)
    {
        Guid instanceId = Guid.NewGuid();
        Flow flow = ParseFlow(flowDefinition?.DefinitionJson, jsonBroker);

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

    private static Flow ParseFlow(string definitionJson, IJsonBroker jsonBroker) =>
        string.IsNullOrWhiteSpace(definitionJson)
            ? null
            : jsonBroker.ParseJson<Flow>(definitionJson);
}
