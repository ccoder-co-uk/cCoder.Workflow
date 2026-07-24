// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface ICalendarEventEventBroker
{
    string GetCurrentUserId();

    ValueTask RaiseCalendarEventAddEventAsync(EventMessage<CalendarEvent> message);

    ValueTask RaiseCalendarEventUpdateEventAsync(EventMessage<CalendarEvent> message);

    ValueTask RaiseCalendarEventDeleteEventAsync(EventMessage<CalendarEvent> message);
}