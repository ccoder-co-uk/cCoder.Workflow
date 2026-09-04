// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Workflow.Brokers.Events;

internal sealed class WorkflowExecutionEventBroker(
    IServiceProvider serviceProvider)
    : IWorkflowExecutionEventBroker
{
    public ValueTask RaiseWorkflowExecuteEventAsync(
        EventMessage<WorkflowRequest> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "workflow_execute", message: message);
}