// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowHubService
{
    private static void ValidateWorkflowHubConnection(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowHubThreadOnJoin(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowHubThreadOnLeave(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateWorkflowHubMessageOnSend(
        params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input =>
            input is null
            || input is string text
            && string.IsNullOrWhiteSpace(value: text)))
        {
            throw new ValidationException(
                message: "Workflow hub values are required.");
        }
    }
}