// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowTokenService
{
    private static void ValidateWorkflowExecutionTokenOnIssue(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}