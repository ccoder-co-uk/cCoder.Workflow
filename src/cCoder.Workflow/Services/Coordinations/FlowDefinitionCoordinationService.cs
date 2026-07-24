// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class FlowDefinitionCoordinationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    IFlowInstanceDataOrchestrationService flowInstanceDataOrchestrationService)
    : IFlowDefinitionCoordinationService
{
    public ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); await ExecuteHandleFlowDefinitionDeleteAsync(flowDefinition: flowDefinition); }, isValueTask: true);

    private async ValueTask ExecuteHandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition)
    {
        IEnumerable<FlowInstanceData> instancesToDelete = flowInstanceDataOrchestrationService
            .GetAll(ignoreFilters: true)
            .Where(predicate: instance => instance.FlowDefinitionId == flowDefinition.Id)
            .ToArray();

        await flowInstanceDataOrchestrationService.DeleteAllFlowInstanceDataAsync(deletedItems: instancesToDelete);
    }

    public ValueTask<Guid> QueueAsync(Guid flowDefinitionId, string asUserId, string args) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinitionId, asUserId, args]); return await ExecuteQueueAsync(flowDefinitionId: flowDefinitionId, asUserId: asUserId, args: args); }, isValueTask: true);

    private async ValueTask<Guid> ExecuteQueueAsync(Guid flowDefinitionId, string asUserId, string args)
    {
        FlowDefinition flowDefinition =
            flowDefinitionOrchestrationService
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: foundFlowDefinition => foundFlowDefinition.Id == flowDefinitionId);

        flowDefinitionOrchestrationService.AuthorizeFlowDefinitionExecution(
            userId: asUserId,
            appId: flowDefinition?.AppId);

        FlowInstanceData flowInstance =
            flowDefinitionOrchestrationService.CreateFlowDefinitionQueuedFlowInstanceData(
                flowDefinition: flowDefinition,
                caller: asUserId,
                args: args);

        flowInstance = await flowInstanceDataOrchestrationService
            .AddQueuedFlowInstanceDataAsync(newEntity: flowInstance);

        return flowInstance.Id;
    }
}