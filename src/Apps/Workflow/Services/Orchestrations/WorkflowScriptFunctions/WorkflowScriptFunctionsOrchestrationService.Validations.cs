// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Workflow.Services.Orchestrations.WorkflowScriptFunctions;

internal sealed partial class WorkflowScriptFunctionsOrchestrationService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ValidationException(
                message: "A workflow script function input is required.");
        }
    }
}