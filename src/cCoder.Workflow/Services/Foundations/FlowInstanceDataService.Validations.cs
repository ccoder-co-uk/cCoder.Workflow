// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowInstanceDataService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowInstanceDataOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedFlowInstanceDataOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowInstanceDataOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Authorize(bool isAuthorized)
    {
        if (!isAuthorized)
        {
            throw new System.Security.SecurityException(message: "Access Denied!");
        }
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}