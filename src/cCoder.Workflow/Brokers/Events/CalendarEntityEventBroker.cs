// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public class CalendarEntityEventBroker(IEventHub eventHub) : ICalendarEntityEventBroker
{
    public ValueTask RaiseCalendarAddEventAsync(EventMessage<Calendar> message) =>
        eventHub.RaiseEventAsync(name:"calendar_add", message:message);

    public ValueTask RaiseCalendarUpdateEventAsync(EventMessage<Calendar> message) =>
        eventHub.RaiseEventAsync(name:"calendar_update", message:message);

    public ValueTask RaiseCalendarDeleteEventAsync(EventMessage<Calendar> message) =>
        eventHub.RaiseEventAsync(name:"calendar_delete", message:message);
}