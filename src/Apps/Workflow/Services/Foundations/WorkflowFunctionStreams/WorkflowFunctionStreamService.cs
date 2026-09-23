// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using Workflow.Brokers.WorkflowFunctionStreams;

namespace Workflow.Services.Foundations.WorkflowFunctionStreams;

internal sealed partial class WorkflowFunctionStreamService(
    IWorkflowFunctionStreamBroker workflowFunctionStreamBroker)
        : IWorkflowFunctionStreamService
{
    public ValueTask<string> ReadHttpRequestDataBodyAsync(
        HttpRequestData request) =>
        TryCatch(
            operation: () =>
            {
                ValidateInputs(inputs: [request]);

                return workflowFunctionStreamBroker.ReadBodyAsync(
                    request: request);
            },
            isValueTask: true);
}