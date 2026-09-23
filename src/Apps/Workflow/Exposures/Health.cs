// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Foundations.WorkflowHttpResponses;

namespace Workflow.Exposures;

internal sealed class Health
{
    private readonly IWorkflowHttpResponseService workflowHttpResponseService;

    public Health(IWorkflowHttpResponseService workflowHttpResponseService) =>
        this.workflowHttpResponseService = workflowHttpResponseService;

    [Function(nameof(Health))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Health")] HttpRequestData request) =>
        workflowHttpResponseService.CreateHttpResponseDataAsync(
            request: request,
            content: "OK");
}