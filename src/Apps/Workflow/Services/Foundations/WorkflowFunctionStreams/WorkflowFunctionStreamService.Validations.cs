// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Workflow.Services.Foundations.WorkflowFunctionStreams;

internal sealed partial class WorkflowFunctionStreamService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ValidationException(
                message: "A workflow function stream input is required.");
        }
    }
}