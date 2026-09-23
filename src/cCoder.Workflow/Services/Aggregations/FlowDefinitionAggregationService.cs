// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class FlowDefinitionAggregationService(
    IFlowDefinitionCoordinationService queueCoordinationService,
    IFlowDefinitionManagementCoordinationService managementCoordinationService)
    : IFlowDefinitionAggregationService
{
    public FlowDefinition GetFlowDefinition(Guid flowDefinitionId) =>
        TryCatch(operation: () =>
        {
            ValidateFlowDefinitionOnGet(inputs: [flowDefinitionId]);
            return managementCoordinationService.GetFlowDefinition(flowDefinitionId: flowDefinitionId);
        });

    public IQueryable<FlowDefinition> GetAllFlowDefinitions() =>
        TryCatch(operation: () =>
        {
            ValidateAllFlowDefinitionsOnGet(inputs: [false]);
            return managementCoordinationService.GetAllFlowDefinitions();
        });

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowDefinitionOnAdd(inputs: [newFlowDefinition]);
            return await managementCoordinationService.AddFlowDefinitionAsync(newFlowDefinition: newFlowDefinition);
        }, isValueTask: true);

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowDefinitionOnUpdate(inputs: [updatedFlowDefinition]);
            return await managementCoordinationService.UpdateFlowDefinitionAsync(updatedFlowDefinition: updatedFlowDefinition);
        }, isValueTask: true);

    public ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowDefinitionOnDelete(inputs: [flowDefinitionId]);
            await managementCoordinationService.DeleteFlowDefinitionAsync(flowDefinitionId: flowDefinitionId);
        }, isValueTask: true);

    public ValueTask<Guid> QueueFlowDefinitionAsync(Guid flowDefinitionId, string asUserId, string args) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowDefinitionOnQueue(inputs: [flowDefinitionId, asUserId, args]);

            return await queueCoordinationService.QueueAsync(
                flowDefinitionId: flowDefinitionId,
                asUserId: asUserId,
                args: args);
        }, isValueTask: true);

    public ValueTask<string> ExecuteScriptAsync(string script) =>
        TryCatch(operation: async () =>
        {
            ValidateScriptOnExecute(inputs: [script]);
            return await managementCoordinationService.ExecuteScriptAsync(script: script);
        }, isValueTask: true);

    public ValueTask<string> ReadRequestBodyAsync(Stream stream) =>
        TryCatch(operation: async () =>
        {
            ValidateRequestBodyOnRead(inputs: [stream]);
            return await managementCoordinationService.ReadRequestBodyAsync(stream: stream);
        }, isValueTask: true);
}