// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Workflow.Services.Foundations.WorkflowHttpResponses;

internal sealed partial class WorkflowHttpResponseService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ValidationException(
                message: "A workflow HTTP response input is required.");
        }
    }
}