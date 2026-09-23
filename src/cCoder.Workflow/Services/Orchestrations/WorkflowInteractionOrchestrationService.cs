// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowInteractionOrchestrationService(
    IWorkflowConfigurationService configurationService,
    IWorkflowRequestBodyService requestBodyService,
    IWorkflowScriptExecutionService scriptExecutionService)
    : IWorkflowInteractionOrchestrationService
{
    public ValueTask<string> ExecuteScriptAsync(string script) =>
        TryCatch(
            operation: async () =>
            {
                ValidateScriptOnExecute(inputs: [script]);

                return await scriptExecutionService.ExecuteAsync(
                    serviceUrl: configurationService.GetServiceUrl(),
                    script: script);
            },
            isValueTask: true);

    public ValueTask<string> ReadRequestBodyAsync(Stream stream) =>
        TryCatch(
            operation: async () =>
            {
                ValidateRequestBodyOnRead(inputs: [stream]);
                return await requestBodyService.ReadTextAsync(stream: stream);
            },
            isValueTask: true);
}