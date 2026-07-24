// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class CalendarEventEventBroker(IEventHub eventHub) : ICalendarEventEventBroker
{
    public ValueTask RaiseCalendarEventAddEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync(name:"calendar_add", message:message);

    public ValueTask RaiseCalendarEventUpdateEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync(name:"calendar_update", message:message);

    public ValueTask RaiseCalendarEventDeleteEventAsync(EventMessage<CalendarEvent> message) =>
        eventHub.RaiseEventAsync(name:"calendar_delete", message:message);
}