// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Brokers.Http;
using Workflow.Services.Foundations.WorkflowExecutions;
using Workflow.Services.Foundations.WorkflowFunctionStreams;
using Workflow.Services.Foundations.WorkflowHttpResponses;

namespace Workflow.Services.Orchestrations.WorkflowFunctions;

internal sealed partial class WorkflowFunctionsOrchestrationService(
    IWorkflowFunctionStreamService workflowFunctionStreamService,
    IWorkflowExecutionService workflowExecutionService,
    IWorkflowHttpResponseService workflowHttpResponseService)
        : IWorkflowFunctionsOrchestrationService
{
    public Task<HttpResponseData> ProcessExecuteAsync(HttpRequestData request) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request]);

            string json =
                await workflowFunctionStreamService
                    .ReadHttpRequestDataBodyAsync(request: request);

            await workflowExecutionService.RunWorkflowRequestAsync(
                workflowRequestJson: json);

            return await workflowHttpResponseService
                .CreateHttpResponseDataAsync(
                request: request,
                content: "OK");
        });
}