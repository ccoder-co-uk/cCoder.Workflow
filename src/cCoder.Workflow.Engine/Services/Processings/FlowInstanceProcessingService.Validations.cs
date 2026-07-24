// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowInstanceProcessingService
{
    private static void ValidateInputs(
        params object[] inputs)
    {
        if (inputs.FirstOrDefault() is not FlowExecution flowExecution
            || flowExecution.Request is null)
        {
            throw new ValidationException(
                message: "A workflow execution request is required.");
        }
    }
}