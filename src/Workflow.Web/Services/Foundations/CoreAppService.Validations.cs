// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.Web.Dependencies;

namespace Workflow.Web.Services.Foundations;

internal sealed partial class CoreAppService
{
    private static void ValidateAppOnGet(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}