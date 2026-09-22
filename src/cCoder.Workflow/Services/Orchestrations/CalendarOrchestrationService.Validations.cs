// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class CalendarOrchestrationService
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