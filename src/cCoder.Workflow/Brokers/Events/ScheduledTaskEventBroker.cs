// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class ScheduledTaskEventBroker(IEventHub eventHub) : IScheduledTaskEventBroker
{
    public ValueTask RaiseScheduledTaskAddEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name:"scheduled_task_add", message:message);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name:"scheduled_task_update", message:message);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name:"scheduled_task_delete", message:message);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(EventMessage<ScheduledTask> message) =>
        eventHub.RaiseEventAsync(name:"scheduled_task_execute", message:message);
}