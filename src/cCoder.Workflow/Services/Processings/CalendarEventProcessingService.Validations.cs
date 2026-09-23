// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class CalendarEventProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarEventOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarEventOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForAppCalendarEventOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateCalendarEventOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCalendarEventOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}