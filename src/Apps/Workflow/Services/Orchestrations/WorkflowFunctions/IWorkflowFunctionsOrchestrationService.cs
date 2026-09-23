// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.Services.Orchestrations.WorkflowFunctions;

internal interface IWorkflowFunctionsOrchestrationService
{
    Task<HttpResponseData> ProcessExecuteAsync(HttpRequestData request);

}