// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.Web.Dependencies;

namespace Workflow.Web.Services.Foundations;

internal sealed partial class CoreAppService
{
    private static void ValidateAppOnGet(
        params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}