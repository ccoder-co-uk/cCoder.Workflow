// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Foundations.WorkflowFunctions;

namespace Workflow.Exposures;

internal sealed class Health
{
    private readonly IWorkflowFunctionsService workflowFunctionsService;

    public Health(IWorkflowFunctionsService workflowFunctionsService) =>
        this.workflowFunctionsService = workflowFunctionsService;

    [Function(nameof(Health))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Health")] HttpRequestData request) =>
        workflowFunctionsService.ProcessHealthAsync(request: request);
}