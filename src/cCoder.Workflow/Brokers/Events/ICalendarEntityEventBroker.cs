using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface ICalendarEntityEventBroker
{
    ValueTask RaiseCalendarAddEventAsync(EventMessage<Calendar> message);
    ValueTask RaiseCalendarUpdateEventAsync(EventMessage<Calendar> message);
    ValueTask RaiseCalendarDeleteEventAsync(EventMessage<Calendar> message);
}







