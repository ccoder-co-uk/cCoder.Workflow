// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Workflow.Dependencies;
using System.Text;
using cCoder.Data;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class FlowDefinitionAggregationService(
    IFlowDefinitionServiceProviderBroker serviceProviderBroker)
    : IFlowDefinitionAggregationService
{
    public FlowDefinition GetFlowDefinition(Guid flowDefinitionId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [flowDefinitionId]);

            return GetFlowDefinitionOrchestrationService()
                .Get(flowDefinitionId: flowDefinitionId);
        });

    public IQueryable<FlowDefinition> GetAllFlowDefinitions() =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: []);

            return GetFlowDefinitionOrchestrationService()
                .GetAll();
        });

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newEntity) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [newEntity]);

                return await GetFlowDefinitionOrchestrationService()
                    .AddFlowDefinitionAsync(newEntity: newEntity);
            },
            isValueTask: true);

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedEntity) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [updatedEntity]);

                return await GetFlowDefinitionOrchestrationService()
                    .UpdateFlowDefinitionAsync(updatedEntity: updatedEntity);
            },
            isValueTask: true);

    public ValueTask DeleteFlowDefinitionAsync(Guid flowDefinitionId) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [flowDefinitionId]);

                await GetFlowDefinitionOrchestrationService()
                    .DeleteAsync(flowDefinitionId: flowDefinitionId);
            },
            isValueTask: true);

    public ValueTask<Guid> QueueFlowDefinitionAsync(Guid flowDefinitionId, string asUserId, string args) =>
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

    private ValueTask<Guid> ExecuteQueueFlowDefinitionAsync(
        Guid flowDefinitionId,
        string asUserId,
        string args)
    {
        string callerId = ResolveCallerId(asUserId: asUserId);

        return GetFlowDefinitionCoordinationService()
            .QueueAsync(
                flowDefinitionId: flowDefinitionId,
                asUserId: callerId,
                args: args);
    }

    public ValueTask<string> ExecuteScriptAsync(string script) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [script]);
                return await ExecuteScriptRequestAsync(script: script);
            },
            isValueTask: true);

    private async ValueTask<string> ExecuteScriptRequestAsync(string script)
    {
        using WorkflowHttpClientDependency api = new(
            apiRoot: GetConfiguration().ServiceUrl,
            timeout: TimeSpan.FromMinutes(minutes: 10));

        return await api.PostTextAsync(
            requestUri: "ExecuteScript",
            content: script);
    }

    private string ResolveCallerId(string asUserId)
    {
        if (!string.IsNullOrWhiteSpace(value: asUserId) && !string.Equals(a: asUserId, b: "Guest", comparisonType: StringComparison.Ordinal))
        {
            return asUserId;
        }

        return GetAuthorizationBroker()
            .GetCurrentUser()?.Id ?? "Guest";
    }

    private IFlowDefinitionOrchestrationService GetFlowDefinitionOrchestrationService() =>
        serviceProviderBroker.GetOperationService<IFlowDefinitionOrchestrationService>(
            operation: FlowDefinitionOperation.Crud);

    private IFlowDefinitionCoordinationService GetFlowDefinitionCoordinationService() =>
        serviceProviderBroker.GetOperationService<IFlowDefinitionCoordinationService>(
            operation: FlowDefinitionOperation.Queue);

    private IAuthorizationBroker GetAuthorizationBroker() =>
        serviceProviderBroker.GetOperationService<IAuthorizationBroker>(
            operation: FlowDefinitionOperation.Authorization);

    private WorkflowConfiguration GetConfiguration() =>
        serviceProviderBroker.GetOperationService<WorkflowConfiguration>(
            operation: FlowDefinitionOperation.Configuration);
}