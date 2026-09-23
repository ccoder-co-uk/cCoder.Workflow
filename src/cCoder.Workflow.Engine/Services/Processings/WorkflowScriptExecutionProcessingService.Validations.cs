// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class WorkflowScriptExecutionProcessingService
{
    private static void ValidateInputs(
        params object[] inputs)
    {
        if (inputs.FirstOrDefault() is not string payload
            || string.IsNullOrWhiteSpace(value: payload))
        {
            throw new ValidationException(
                message: "A workflow script payload is required.");
        }
    }
}