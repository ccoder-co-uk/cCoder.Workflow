// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowEventService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowEventOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowEventOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSubscriptionsOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppIdForWorkflowEventOnGet(params object[] inputs) =>
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