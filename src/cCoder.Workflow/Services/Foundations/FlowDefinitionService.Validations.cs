// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowDefinitionService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWithInstancesOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWithInstancesByAppIdOnDelete(params object[] inputs) =>
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