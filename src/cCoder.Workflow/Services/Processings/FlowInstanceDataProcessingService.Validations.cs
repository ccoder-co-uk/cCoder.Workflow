// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowInstanceDataProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowInstanceDataOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedFlowInstanceDataOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowInstanceDataOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFlowInstanceDataOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFlowInstanceDataOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}