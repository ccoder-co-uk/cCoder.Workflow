// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class ScheduledTaskProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDueScheduledTasksOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScheduledTaskOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScheduledTaskOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateScheduledTaskOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllScheduledTaskOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}