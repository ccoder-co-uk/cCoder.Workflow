// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class CalendarService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCalendarOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForAppCalendarOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Authorize(bool isAuthorized)
    {
        if (!isAuthorized)
        {
            throw new System.Security.SecurityException(message: "Access Denied!");
        }
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}