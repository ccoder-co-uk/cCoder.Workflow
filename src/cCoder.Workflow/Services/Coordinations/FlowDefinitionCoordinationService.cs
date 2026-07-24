// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class FlowDefinitionCoordinationService(
    IFlowQueueOrchestrationService flowQueueOrchestrationService,
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

    private ValueTask<Guid> ExecuteQueueAsync(Guid flowDefinitionId, string asUserId, string args) =>
        flowQueueOrchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: asUserId,
            args: args);
}