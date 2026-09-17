// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowInstanceService
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