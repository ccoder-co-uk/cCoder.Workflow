using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface ICalendarEventEventBroker
{
    ValueTask RaiseCalendarEventAddEventAsync(EventMessage<CalendarEvent> message);
    ValueTask RaiseCalendarEventUpdateEventAsync(EventMessage<CalendarEvent> message);
    ValueTask RaiseCalendarEventDeleteEventAsync(EventMessage<CalendarEvent> message);
}







