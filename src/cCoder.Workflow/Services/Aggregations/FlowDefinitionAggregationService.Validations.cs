// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class FlowDefinitionAggregationService
{
    private static void ValidateFlowDefinitionOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFlowDefinitionsOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFlowDefinitionOnQueue(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnExecute(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRequestBodyOnRead(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}