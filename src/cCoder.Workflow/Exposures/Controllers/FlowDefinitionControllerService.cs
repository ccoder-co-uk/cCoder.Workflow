// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using cCoder.Data;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Exposures.Controllers;

public sealed class FlowDefinitionControllerService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    IFlowDefinitionCoordinationService flowDefinitionCoordinationService,
    IWorkflowMetadataTypeService workflowMetadataTypeService,
    IAuthorizationBroker authorizationBroker,
    Config config)
    : IFlowDefinitionControllerService
{
    public FlowDefinition Get(Guid flowDefinitionId) =>
        flowDefinitionOrchestrationService.Get(flowDefinitionId: flowDefinitionId);

    public IQueryable<FlowDefinition> GetAll() =>
        flowDefinitionOrchestrationService.GetAll();

    public ValueTask<FlowDefinition> PostFlowDefinitionAsync(FlowDefinition newEntity) =>
        flowDefinitionOrchestrationService.AddFlowDefinitionAsync(newEntity: newEntity);

    public ValueTask<FlowDefinition> PutFlowDefinitionAsync(FlowDefinition updatedEntity) =>
        flowDefinitionOrchestrationService.UpdateFlowDefinitionAsync(updatedEntity: updatedEntity);

    public ValueTask DeleteAsync(Guid flowDefinitionId) =>
        flowDefinitionOrchestrationService.DeleteAsync(flowDefinitionId: flowDefinitionId);

    public ValueTask<Guid> PostFlowDefinitionQueueAsync(Guid flowDefinitionId, string asUserId, string args)
    {
        string callerId = ResolveCallerId(asUserId: asUserId);
        return flowDefinitionCoordinationService.QueueAsync(flowDefinitionId: flowDefinitionId, asUserId: callerId, args: args);
    }

    public async Task<string> PostScriptAsync(string script)
    {
        using HttpClient api = new(
            new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            }
        )
        {
            BaseAddress = new Uri(config.Services["Workflow"]),
            Timeout = TimeSpan.FromMinutes(minutes: 10),
        };

        HttpResponseMessage response = await api.PostAsync(
requestUri: "ExecuteScript",
content: new StringContent(script, Encoding.UTF8, "text/plain")
        );

        return await response.Content.ReadAsStringAsync();
    }

    public MetadataContainerSet[] GetKnownActivityTypes() =>
        workflowMetadataTypeService.GetKnownActivityTypes();

    public MetadataContainerSet[] GetKnownSystemTypes() =>
        workflowMetadataTypeService.GetKnownSystemTypes();

    private string ResolveCallerId(string asUserId)
    {
        if (!string.IsNullOrWhiteSpace(value: asUserId) && !string.Equals(a: asUserId, b: "Guest", comparisonType: StringComparison.Ordinal))
        {
            return asUserId;
        }

        return authorizationBroker.GetCurrentUser()?.Id ?? "Guest";
    }
}