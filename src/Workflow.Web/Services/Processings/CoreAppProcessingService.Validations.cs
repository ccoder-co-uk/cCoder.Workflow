// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Workflow.Web.Services.Processings;

internal sealed partial class CoreAppProcessingService
{
    private static void ValidateInputs(
        params object[] inputs)
    {
        if (inputs
            .OfType<int>()
            .Any(predicate: value => value <= 0))
        {
            throw new ValidationException(
                message: "App identifiers must be greater than zero.");
        }
    }
}