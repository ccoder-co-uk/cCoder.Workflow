// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowInstanceService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateOldFlowInstanceDataOnDelete(
        params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateClaimedFlowInstanceDataOnGet(
        params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}