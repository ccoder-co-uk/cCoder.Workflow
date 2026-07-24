// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class ScheduledTaskEventBroker(
    IEventHub eventHub,
    ICoreAuthInfo authInfo)
        : IScheduledTaskEventBroker
{
    public string GetCurrentUserId() =>
        authInfo.SSOUserId;

    public ValueTask RaiseScheduledTaskAddEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name: "scheduled_task_add", message: message);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name: "scheduled_task_update", message: message);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name: "scheduled_task_delete", message: message);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name: "scheduled_task_execute", message: message);
}