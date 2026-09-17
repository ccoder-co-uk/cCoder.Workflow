// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Foundations.WorkflowFunctions;

namespace Workflow.Exposures;

internal sealed class Execute
{
    private readonly IWorkflowFunctionsService workflowFunctionsService;

    public Execute(IWorkflowFunctionsService workflowFunctionsService) =>
        this.workflowFunctionsService = workflowFunctionsService;

    [Function(nameof(Execute))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) =>
        workflowFunctionsService.ProcessExecuteAsync(request: request);
}