// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Orchestrations.WorkflowScriptFunctions;

namespace Workflow.Exposures;

internal sealed class ExecuteScript
{
    private readonly IWorkflowScriptFunctionsOrchestrationService
        workflowScriptFunctionsOrchestrationService;

    public ExecuteScript(
        IWorkflowScriptFunctionsOrchestrationService
            workflowScriptFunctionsOrchestrationService) =>
        this.workflowScriptFunctionsOrchestrationService =
            workflowScriptFunctionsOrchestrationService;

    [Function(nameof(ExecuteScript))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request,
        [FromQuery] bool useDetails = false) =>
        workflowScriptFunctionsOrchestrationService.ProcessExecuteScriptAsync(
            request: request,
            useDetails: useDetails);
}