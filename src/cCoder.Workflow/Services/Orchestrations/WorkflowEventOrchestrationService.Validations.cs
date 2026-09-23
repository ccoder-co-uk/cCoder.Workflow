// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowEventOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowEventSubscriptionsOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
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