// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Support;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Workflow;

public sealed class Execute(IFlowRunner flowRunner)
{
    [Function(nameof(Execute))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        string json = await new StreamReader(request.Body).ReadToEndAsync();
        WorkflowRequest workflowRequest = JsonConvert.DeserializeObject<WorkflowRequest>(value:json, settings:WorkflowJson.GetJsonSettings())
            ?? throw new InvalidOperationException("Workflow request payload could not be deserialized.");

        await flowRunner.RunAsync(request:workflowRequest);

        HttpResponseData response = request.CreateResponse(statusCode:System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(value:"OK");
        return response;
    }
}