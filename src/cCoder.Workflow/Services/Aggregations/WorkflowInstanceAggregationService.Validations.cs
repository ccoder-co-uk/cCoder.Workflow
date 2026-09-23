// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class WorkflowInstanceAggregationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}