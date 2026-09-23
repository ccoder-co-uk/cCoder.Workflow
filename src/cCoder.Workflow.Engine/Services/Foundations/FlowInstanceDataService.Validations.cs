// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowInstanceDataService
{
    private static void ValidateFlowInstanceDataOnGet(
        params object[] inputs)
    {
        Validate(inputs: inputs);

        if (inputs
            .OfType<Guid>()
            .FirstOrDefault() == Guid.Empty)
        {
            throw new ValidationException(
                message: "A workflow instance identifier is required.");
        }
    }

    private static void ValidateFlowInstanceDataOnSave(
        params object[] inputs)
    {
        Validate(inputs: inputs);

        if (inputs.FirstOrDefault() is not FlowInstanceData)
        {
            throw new ValidationException(
                message: "A workflow instance is required.");
        }
    }

    private static void Validate(params object[] inputs)
    {
        if (inputs
            .OfType<string>()
            .Any(predicate: value => string.IsNullOrWhiteSpace(value: value)))
        {
            throw new ValidationException(
                message: "Workflow API and authentication values are required.");
        }
    }
}