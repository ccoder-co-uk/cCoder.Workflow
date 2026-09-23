// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using Workflow.Brokers.WorkflowExecutions;
using Workflow.Brokers.WorkflowJson;

namespace Workflow.Services.Foundations.WorkflowExecutions;

internal sealed partial class WorkflowExecutionService(
    IWorkflowExecutionBroker workflowExecutionBroker,
    IWorkflowJsonBroker workflowJsonBroker)
        : IWorkflowExecutionService
{
    public Task RunWorkflowRequestAsync(string workflowRequestJson) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [workflowRequestJson]);

            WorkflowRequest workflowRequest =
                workflowJsonBroker.Deserialize<WorkflowRequest>(
                    value: workflowRequestJson)
                ?? throw new InvalidOperationException(
                    message: "Workflow request payload could not be deserialized.");

            return workflowExecutionBroker.RunWorkflowRequestAsync(
                workflowRequest: workflowRequest);
        });
}