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
            .GetAll(ignoreFilters:true)
            .Where(predicate:instance => instance.FlowDefinitionId == flowDefinition.Id)
            .ToArray();

        await flowInstanceDataOrchestrationService.DeleteAllAsync(items:instancesToDelete);
    }

    public async ValueTask<Guid> QueueAsync(Guid id, string asUserId, string args)
    {
        FlowDefinition flowDefinition =
            flowDefinitionOrchestrationService
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate:foundFlowDefinition => foundFlowDefinition.Id == id);

        authorizationBroker.Authorize(
userId:            asUserId, 
appId:            flowDefinition?.AppId, 
privilege:            "flowdefinition_execute");

        FlowInstanceData flowInstance = 
            CreateFlowInstanceData(flowDefinition:flowDefinition, caller:asUserId, args:args);

        flowInstance = await flowInstanceDataOrchestrationService
            .AddQueuedAsync(entity:flowInstance);

        return flowInstance.Id;
    }

    private FlowInstanceData CreateFlowInstanceData(FlowDefinition flowDefinition, string caller, string args)
    {
        if (flowDefinition == null)
            throw new SecurityException("Access Denied!");

        if (string.IsNullOrWhiteSpace(value:caller))
            throw new SecurityException("Access Denied!");

        Guid instanceId = Guid.NewGuid();
        Flow flow = ParseFlow(definitionJson:flowDefinition.DefinitionJson);

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

        start.Data = jsonBroker.ParseJson(json:args);

        return new FlowInstanceData
        {
            Id = instanceId,
            State = "Queued",
            FlowDefinitionId = flowDefinition.Id,
            Start = DateTimeOffset.UtcNow,
            Caller = caller,
            ContextString = jsonBroker.Serialize(value:context),
            FlowDefinition = flowDefinition
        };
    }

    private Flow ParseFlow(string definitionJson) =>
        string.IsNullOrWhiteSpace(value:definitionJson)
            ? null
            : jsonBroker.ParseJson<Flow>(json:definitionJson);
}