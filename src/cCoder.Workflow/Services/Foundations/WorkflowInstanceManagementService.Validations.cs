// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowInstanceManagementService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOldFlowInstanceDataOnDelete(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateClaimedFlowInstanceDataOnGet(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}