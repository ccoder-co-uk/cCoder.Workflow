// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Orchestrations.WorkflowFunctions;

namespace Workflow.Exposures;

internal sealed class Execute
{
    private readonly IWorkflowFunctionsOrchestrationService
        workflowFunctionsOrchestrationService;

    public Execute(
        IWorkflowFunctionsOrchestrationService
            workflowFunctionsOrchestrationService) =>
        this.workflowFunctionsOrchestrationService =
            workflowFunctionsOrchestrationService;

    [Function(nameof(Execute))]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request) =>
        workflowFunctionsOrchestrationService.ProcessExecuteAsync(
            request: request);
}