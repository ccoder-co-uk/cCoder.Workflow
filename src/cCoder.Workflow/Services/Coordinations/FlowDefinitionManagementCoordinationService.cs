// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class FlowDefinitionManagementCoordinationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    IWorkflowInteractionOrchestrationService interactionOrchestrationService)
    : IFlowDefinitionManagementCoordinationService
{
    public FlowDefinition GetFlowDefinition(Guid flowDefinitionId) =>
        TryCatch(operation: () =>
        {
            ValidateFlowDefinitionOnGet(inputs: [flowDefinitionId]);
            return flowDefinitionOrchestrationService.Get(flowDefinitionId: flowDefinitionId);
        });

    public IQueryable<FlowDefinition> GetAllFlowDefinitions() =>
        TryCatch(operation: () => flowDefinitionOrchestrationService.GetAll());

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition) =>
        TryCatch(
            operation: async () =>
            {
                ValidateFlowDefinitionOnAdd(inputs: [newFlowDefinition]);

                return await flowDefinitionOrchestrationService.AddFlowDefinitionAsync(
                    newFlowDefinition: newFlowDefinition);
            },
            isValueTask: true);

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition) =>
        TryCatch(
            operation: async () =>
            {
                ValidateFlowDefinitionOnUpdate(inputs: [updatedFlowDefinition]);

                return await flowDefinitionOrchestrationService.UpdateFlowDefinitionAsync(
                    updatedFlowDefinition: updatedFlowDefinition);
            },
            isValueTask: true);

    public ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId) =>
        TryCatch(
            operation: async () =>
            {
                ValidateFlowDefinitionOnDelete(inputs: [flowDefinitionId]);

                await flowDefinitionOrchestrationService.DeleteAsync(
                    flowDefinitionId: flowDefinitionId);
            },
            isValueTask: true);

    public ValueTask<string> ExecuteScriptAsync(string script) =>
        TryCatch(
            operation: async () =>
            {
                ValidateScriptOnExecute(inputs: [script]);
                return await interactionOrchestrationService.ExecuteScriptAsync(script: script);
            },
            isValueTask: true);

    public ValueTask<string> ReadRequestBodyAsync(Stream stream) =>
        TryCatch(
            operation: async () =>
            {
                ValidateRequestBodyOnRead(inputs: [stream]);
                return await interactionOrchestrationService.ReadRequestBodyAsync(stream: stream);
            },
            isValueTask: true);
}