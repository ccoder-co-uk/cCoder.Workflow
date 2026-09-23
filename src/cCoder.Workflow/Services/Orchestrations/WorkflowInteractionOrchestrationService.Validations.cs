// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowInteractionOrchestrationService
{
    private static void ValidateScriptOnExecute(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRequestBodyOnRead(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}