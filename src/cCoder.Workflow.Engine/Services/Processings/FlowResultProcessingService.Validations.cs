// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowResultProcessingService
{
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