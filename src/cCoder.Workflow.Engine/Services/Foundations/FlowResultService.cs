// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowResultService(
    IWorkflowHttpClientBroker workflowHttpClientBroker,
    IJsonBroker jsonBroker)
    : IFlowResultService
{
    public T Deserialize<T>(string value) =>
        TryCatch(operation: () =>
        {
            ValidateSerializationInput(input: value);

            return jsonBroker.Deserialize<T>(value: value);
        });

    public string Serialize(object value) =>
        TryCatch(operation: () =>
        {
            ValidateSerializationInput(input: value);

            return jsonBroker.Serialize(value: value);
        });

    public ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(
                inputs:
                [
                    flowInstanceData,
                    apiRoot,
                    authToken
                ]);

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
                    $"Workflow result save failed with status "
                    + $"{response.StatusCode} "
                    + $"({response.Status})."
                    + $"{Environment.NewLine}Payload:"
                    + $"{Environment.NewLine}{payload}"
                    + $"{Environment.NewLine}Response:"
                    + $"{Environment.NewLine}{response.Body}");
            }
        });

}