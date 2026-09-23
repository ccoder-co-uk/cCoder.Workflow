// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowInstanceDataService(
    IWorkflowHttpClientBroker workflowHttpClientBroker,
    IJsonBroker jsonBroker)
    : IFlowInstanceDataService
{
    public Task<FlowInstanceData> GetFlowInstanceDataAsync(
        string apiRoot,
        string authToken,
        Guid flowInstanceDataId) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowInstanceDataOnGet(
                inputs: [apiRoot, authToken, flowInstanceDataId]);

            string rawInstance = await workflowHttpClientBroker.GetStringAsync(
                apiRoot: apiRoot,
                authToken: authToken,
                requestUri:
                    $"Workflow/FlowInstanceData({flowInstanceDataId})"
                    + "?$expand=FlowDefinition($expand=App)");

            return jsonBroker.Deserialize<FlowInstanceData>(
                value: rawInstance)
                ?? throw new InvalidOperationException(
                    "Workflow instance response was empty.");
        });

    public ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken) =>
        TryCatch(operation: async () =>
        {
            ValidateFlowInstanceDataOnSave(
                inputs: [flowInstanceData, apiRoot, authToken]);

            string payload = jsonBroker.SerializeForOData(
                value: new
                {
                    flowInstanceData.Id,
                    flowInstanceData.FlowDefinitionId,
                    flowInstanceData.Name,
                    flowInstanceData.State,
                    flowInstanceData.ReportingComponentName,
                    flowInstanceData.Caller,
                    flowInstanceData.ContextString,
                    flowInstanceData.Start,
                    flowInstanceData.End
                });

            WorkflowHttpResult response =
                await workflowHttpClientBroker.PutJsonAsync(
                    apiRoot: apiRoot,
                    authToken: authToken,
                    requestUri:
                        $"Workflow/FlowInstanceData"
                        + $"({flowInstanceData.Id})",
                    payload: payload);

            if (!response.IsSuccess)
            {
                throw new WorkflowEngineServiceException(
                    $"Workflow state save failed with status "
                    + $"{response.StatusCode} ({response.Status})."
                    + Environment.NewLine
                    + response.Body);
            }
        });
}