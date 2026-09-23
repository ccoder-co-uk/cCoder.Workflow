// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class FlowQueueOrchestrationService(
    IAuthorizationBroker authorizationBroker,
    IFlowDefinitionProcessingService flowDefinitionProcessingService,
    IFlowInstanceDataProcessingService flowInstanceDataProcessingService,
    IFlowInstanceDataEventProcessingService flowInstanceDataEventProcessingService)
    : IFlowQueueOrchestrationService
{
    public ValueTask<Guid> QueueFlowDefinitionAsync(
        Guid flowDefinitionId,
        string asUserId,
        string args) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [flowDefinitionId, asUserId, args]);

                return await ExecuteQueueFlowDefinitionAsync(
                    flowDefinitionId: flowDefinitionId,
                    asUserId: asUserId,
                    args: args);
            },
            isValueTask: true);

    private async ValueTask<Guid> ExecuteQueueFlowDefinitionAsync(
        Guid flowDefinitionId,
        string asUserId,
        string args)
    {
        string callerId = ResolveCallerId(asUserId: asUserId);

        FlowDefinition flowDefinition = flowDefinitionProcessingService
            .GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: foundFlowDefinition => foundFlowDefinition.Id == flowDefinitionId);

        _ = flowDefinitionProcessingService.AuthorizeFlowDefinitionExecution(
            userId: callerId,
            appId: flowDefinition?.AppId);

        FlowInstanceData flowInstance = CreateFlowInstanceData(
            flowDefinition: flowDefinition,
            caller: callerId,
            args: args);

        flowInstance = await flowInstanceDataProcessingService
            .AddQueuedFlowInstanceDataAsync(newFlowInstanceData: flowInstance);

        await flowInstanceDataEventProcessingService
            .RaiseFlowInstanceDataAddEventAsync(flowInstanceData: flowInstance);

        return flowInstance.Id;
    }

    private string ResolveCallerId(string asUserId)
    {
        if (!string.IsNullOrWhiteSpace(value: asUserId)
            && !string.Equals(
                a: asUserId,
                b: "Guest",
                comparisonType: StringComparison.Ordinal))
        {
            return asUserId;
        }

        return authorizationBroker.GetCurrentUser()?.Id ?? "Guest";
    }

    private FlowInstanceData CreateFlowInstanceData(
        FlowDefinition flowDefinition,
        string caller,
        string args)
    {
        if (flowDefinition == null || string.IsNullOrWhiteSpace(value: caller))
        {
            throw new SecurityException("Access Denied!");
        }

        Guid instanceId = Guid.NewGuid();

        Flow flow = flowDefinitionProcessingService
            .ParseFlowDefinition(definitionJson: flowDefinition.DefinitionJson) as Flow;

        if (flow == null)
        {
            throw new InvalidOperationException("Flow definition does not contain a valid workflow definition.");
        }

        WorkflowContext context = new()
        {
            ExecutionState = "Queued",
            InstanceId = instanceId,
            Flow = flow,
            Variables = new Dictionary<string, object> { { "Data", args } },
            ExecutionLog = Array.Empty<WorkflowLogEntry>()
        };

        Start start = context.Flow.Activities.OfType<Start>()
            .FirstOrDefault();

        if (start == null)
        {
            throw new InvalidOperationException("Flow definition does not contain a Start activity.");
        }

        start.Data = flowDefinitionProcessingService.ParseFlowDefinitionData(args: args);

        return new FlowInstanceData
        {
            Id = instanceId,
            State = "Queued",
            FlowDefinitionId = flowDefinition.Id,
            Start = DateTimeOffset.UtcNow,
            Caller = caller,
            ContextString = flowDefinitionProcessingService.SerializeFlowDefinitionContext(context: context),
            FlowDefinition = flowDefinition
        };
    }
}