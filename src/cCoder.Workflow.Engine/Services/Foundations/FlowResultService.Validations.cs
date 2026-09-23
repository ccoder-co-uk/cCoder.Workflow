// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowResultService
{
    private static void ValidateSerializationInput(object input)
    {
        if (input is null ||
            input is string text && string.IsNullOrWhiteSpace(value: text))
        {
            throw new ValidationException(
                message: "A serialization value is required.");
        }
    }

    private static void ValidateInputs(
        params object[] inputs)
    {
        if (inputs.FirstOrDefault() is not FlowInstanceData)
        {
            throw new ValidationException(
                message: "A workflow instance result is required.");
        }

        if (inputs
            .OfType<string>()
            .Any(predicate: value =>
                string.IsNullOrWhiteSpace(value: value)))
        {
            throw new ValidationException(
                message:
                    "Workflow API and authentication values "
                    + "are required.");
        }
    }
}