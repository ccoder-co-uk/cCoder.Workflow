using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface IScheduledTaskEventBroker
{
    ValueTask RaiseScheduledTaskAddEventAsync(EventMessage<ScheduledTask> message);
    ValueTask RaiseScheduledTaskUpdateEventAsync(EventMessage<ScheduledTask> message);
    ValueTask RaiseScheduledTaskDeleteEventAsync(EventMessage<ScheduledTask> message);
    ValueTask RaiseScheduledTaskExecuteEventAsync(EventMessage<ScheduledTask> message);
}







