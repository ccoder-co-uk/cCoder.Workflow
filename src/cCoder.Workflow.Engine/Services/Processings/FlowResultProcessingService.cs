// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Support;
using Newtonsoft.Json;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowResultProcessingService
    : IFlowResultProcessingService
{
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

            using WorkflowHttpClientDependency api =
                new(
                    apiRoot: apiRoot,
                    authToken: authToken);

            string payload = JsonConvert.SerializeObject(
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
                },
                formatting: Formatting.None);

            WorkflowHttpResult response = await api.PutJsonAsync(
                requestUri:
                    $"Workflow/FlowInstanceData"
                    + $"({flowInstanceData.Id})",
                payload: payload);

            if (!response.IsSuccess)
            {
                throw new HttpRequestException(
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