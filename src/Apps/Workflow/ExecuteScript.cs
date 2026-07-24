// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow;

public sealed class ExecuteScript(IWorkflowScriptExecutionService scriptExecutionService)
{
    [Function(nameof(ExecuteScript))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request,
        [FromQuery] bool useDetails = false)
    {
        string payload = await new StreamReader(request.Body).ReadToEndAsync();
        string result = await scriptExecutionService.ExecuteAsync(payload:payload, useDetails:useDetails);

        HttpResponseData response = request.CreateResponse(statusCode:System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(value:result);
        return response;
    }
}