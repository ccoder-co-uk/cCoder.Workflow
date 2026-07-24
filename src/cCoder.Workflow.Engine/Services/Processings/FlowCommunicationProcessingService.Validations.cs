// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowCommunicationProcessingService
{
    private static void ValidateInputs(
        params object[] inputs)
    {
        if (inputs.FirstOrDefault() is not WorkflowRequest
            workflowRequest
            || workflowRequest.InstanceId == Guid.Empty
            || string.IsNullOrWhiteSpace(
                value: workflowRequest.Api))
        {
            throw new ValidationException(
                message: "A valid workflow request is required.");
        }
    }
}