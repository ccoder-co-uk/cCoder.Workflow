// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class CalendarEntityEventService
{
    private static void ValidateInputs(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}