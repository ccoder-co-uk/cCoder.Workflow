// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowInstanceDataEventProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}