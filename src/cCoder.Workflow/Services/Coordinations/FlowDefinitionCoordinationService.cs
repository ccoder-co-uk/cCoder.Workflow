// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal class FlowDefinitionCoordinationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    IFlowInstanceDataOrchestrationService flowInstanceDataOrchestrationService,
    IAuthorizationBroker authorizationBroker,
    IJsonBroker jsonBroker)
    : IFlowDefinitionCoordinationService
{
    public async ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition)
    {
        IEnumerable<FlowInstanceData> instancesToDelete = flowInstanceDataOrchestrationService
            .GetAll(true)
            .Where(instance => instance.FlowDefinitionId == flowDefinition.Id)
            .ToArray();

        await flowInstanceDataOrchestrationService.DeleteAllAsync(instancesToDelete);
    }

    public async ValueTask<Guid> QueueAsync(Guid id, string asUserId, string args)
    {
        FlowDefinition flowDefinition =
            flowDefinitionOrchestrationService
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(foundFlowDefinition => foundFlowDefinition.Id == id);

        authorizationBroker.Authorize(
            asUserId, 
            flowDefinition?.AppId, 
            "flowdefinition_execute");

        FlowInstanceData flowInstance = 
            CreateFlowInstanceData(flowDefinition, asUserId, args);

        flowInstance = await flowInstanceDataOrchestrationService
            .AddQueuedAsync(flowInstance);

        return flowInstance.Id;
    }

    private FlowInstanceData CreateFlowInstanceData(FlowDefinition flowDefinition, string caller, string args)
    {
        if (flowDefinition == null)
            throw new SecurityException("Access Denied!");

        if (string.IsNullOrWhiteSpace(caller))
            throw new SecurityException("Access Denied!");

        Guid instanceId = Guid.NewGuid();
        Flow flow = ParseFlow(flowDefinition.DefinitionJson);

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
}