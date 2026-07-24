// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Brokers.Events;

public interface ICalendarEntityEventBroker
{
    string GetCurrentUserId();

    ValueTask RaiseCalendarAddEventAsync(EventMessage<Calendar> message);

    ValueTask RaiseCalendarUpdateEventAsync(EventMessage<Calendar> message);

    ValueTask RaiseCalendarDeleteEventAsync(EventMessage<Calendar> message);
}