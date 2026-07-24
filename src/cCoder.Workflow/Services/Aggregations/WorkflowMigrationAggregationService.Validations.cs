// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class WorkflowMigrationAggregationService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}