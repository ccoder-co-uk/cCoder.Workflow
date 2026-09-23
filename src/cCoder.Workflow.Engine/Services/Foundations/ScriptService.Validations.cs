// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class ScriptService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Length < 2
            || inputs[0] is not string code
            || string.IsNullOrWhiteSpace(value: code)
            || inputs[1] is not string[] imports
            || imports.Length == 0)
        {
            throw new ValidationException(
                message: "Script code and imports are required.");
        }
    }
}