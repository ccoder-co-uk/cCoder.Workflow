// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class CalendarProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateCalendarOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCalendarOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}