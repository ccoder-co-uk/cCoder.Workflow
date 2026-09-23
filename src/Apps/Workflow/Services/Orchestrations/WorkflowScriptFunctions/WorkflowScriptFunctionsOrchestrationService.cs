// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Services.Foundations.WorkflowFunctionStreams;
using Workflow.Services.Foundations.WorkflowHttpResponses;
using Workflow.Services.Foundations.WorkflowScriptExecutions;

namespace Workflow.Services.Orchestrations.WorkflowScriptFunctions;

internal sealed partial class WorkflowScriptFunctionsOrchestrationService(
    IWorkflowFunctionStreamService workflowFunctionStreamService,
    IWorkflowScriptExecutionService workflowScriptExecutionService,
    IWorkflowHttpResponseService workflowHttpResponseService)
        : IWorkflowScriptFunctionsOrchestrationService
{
    public Task<HttpResponseData> ProcessExecuteScriptAsync(
        HttpRequestData request,
        bool useDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request, useDetails]);

            string payload =
                await workflowFunctionStreamService
                    .ReadHttpRequestDataBodyAsync(request: request);

            string result =
                await workflowScriptExecutionService.ExecuteScriptAsync(
                    payload: payload,
                    useDetails: useDetails);

            return await workflowHttpResponseService
                .CreateHttpResponseDataAsync(
                    request: request,
                    content: result);
        });
}