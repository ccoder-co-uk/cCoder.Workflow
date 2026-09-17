// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Foundations.WorkflowFunctions;

namespace Workflow.Exposures;

internal sealed class ExecuteScript
{
    private readonly IWorkflowFunctionsService workflowFunctionsService;

    public ExecuteScript(IWorkflowFunctionsService workflowFunctionsService) =>
        this.workflowFunctionsService = workflowFunctionsService;

    [Function(nameof(ExecuteScript))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request,
        [FromQuery] bool useDetails = false) =>
        workflowFunctionsService.ProcessExecuteScriptAsync(
            request: request,
            useDetails: useDetails);
}