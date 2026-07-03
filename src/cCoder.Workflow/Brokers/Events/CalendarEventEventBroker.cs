using cCoder.Data.Models.Planning;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class CalendarEventEventBroker(IEventHub eventHub) : ICalendarEventEventBroker
{
    public ValueTask RaiseCalendarEventAddEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync("calendar_add", message);

    public ValueTask RaiseCalendarEventUpdateEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync("calendar_update", message);

    public ValueTask RaiseCalendarEventDeleteEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync("calendar_delete", message);
}







