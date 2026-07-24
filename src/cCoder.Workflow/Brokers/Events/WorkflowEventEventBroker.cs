// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.Extensions.DependencyInjection;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class WorkflowEventEventBroker(
    IServiceProvider serviceProvider)
        : IWorkflowEventEventBroker
{
    public string GetCurrentUserId() =>
        serviceProvider.GetRequiredService<ICoreAuthInfo>().SSOUserId;

    public ValueTask RaiseWorkflowEventAddEventAsync(EventMessage<WorkflowEvent> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "workflow_add", message: message);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(EventMessage<WorkflowEvent> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "workflow_update", message: message);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(EventMessage<WorkflowEvent> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "workflow_delete", message: message);
}