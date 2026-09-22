// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowEventProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSubscriptionsOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowEventOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowEventOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateWorkflowEventOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllWorkflowEventOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}