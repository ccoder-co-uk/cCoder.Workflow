// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowExecutionEventService(
    IWorkflowExecutionEventBroker workflowExecutionEventBroker)
    : IWorkflowExecutionEventService
{
    public ValueTask RaiseWorkflowRequestEventMessageAsync(
        EventMessage<WorkflowRequest> message) =>
        TryCatch(
            operation: async () =>
            {
                ValidateWorkflowRequestEventMessageOnRaise(inputs: [message]);
                await workflowExecutionEventBroker.RaiseWorkflowExecuteEventAsync(message: message);
            },
            isValueTask: true);
}