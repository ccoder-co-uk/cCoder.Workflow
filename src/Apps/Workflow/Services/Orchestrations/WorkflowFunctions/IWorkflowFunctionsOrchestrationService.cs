// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.Services.Orchestrations.WorkflowFunctions;

internal interface IWorkflowFunctionsOrchestrationService
{
    Task<HttpResponseData> ProcessExecuteAsync(HttpRequestData request);

}