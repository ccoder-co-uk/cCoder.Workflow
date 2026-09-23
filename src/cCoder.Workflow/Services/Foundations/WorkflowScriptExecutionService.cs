// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowScriptExecutionService(
    IWorkflowHttpClientBroker httpClientBroker)
    : IWorkflowScriptExecutionService
{
    public ValueTask<string> ExecuteAsync(
        string serviceUrl,
        string script) =>
        TryCatch(
            operation: async () =>
            {
                ValidateInputs(inputs: [serviceUrl, script]);

                return await httpClientBroker.PostTextAsync(
                    apiRoot: serviceUrl,
                    timeout: TimeSpan.FromMinutes(minutes: 10),
                    requestUri: "ExecuteScript",
                    content: script);
            },
            isValueTask: true);
}