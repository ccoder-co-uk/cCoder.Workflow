// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowDefinitionService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnAdd(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnUpdate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateWithInstancesOnDelete(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateWithInstancesByAppIdOnDelete(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}