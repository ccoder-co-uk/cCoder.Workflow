// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace Workflow.Web.Services.Foundations;

internal sealed partial class CoreAppService
{
    private static void ValidateAppOnGet(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}