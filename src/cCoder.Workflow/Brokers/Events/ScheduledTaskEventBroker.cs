// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.Extensions.DependencyInjection;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class ScheduledTaskEventBroker(
    IServiceProvider serviceProvider)
        : IScheduledTaskEventBroker
{
    public string GetCurrentUserId() =>
        serviceProvider.GetRequiredService<ICoreAuthInfo>().SSOUserId;

    public ValueTask RaiseScheduledTaskAddEventAsync(EventMessage<ScheduledTask> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "scheduled_task_add", message: message);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(EventMessage<ScheduledTask> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "scheduled_task_update", message: message);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(EventMessage<ScheduledTask> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "scheduled_task_delete", message: message);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(EventMessage<ScheduledTask> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "scheduled_task_execute", message: message);
}