// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class CalendarEntityEventService(
    ICalendarEntityEventBroker calendarEventBroker,
    ICoreAuthInfo authInfo
) : ICalendarEntityEventService
{
    public async ValueTask RaiseCalendarAddEventAsync(Calendar entity)
    {
        EventMessage<Calendar> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventBroker.RaiseCalendarAddEventAsync(message);
    }

    public async ValueTask RaiseCalendarUpdateEventAsync(Calendar entity)
    {
        EventMessage<Calendar> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventBroker.RaiseCalendarUpdateEventAsync(message);
    }

    public async ValueTask RaiseCalendarDeleteEventAsync(Calendar entity)
    {
        EventMessage<Calendar> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventBroker.RaiseCalendarDeleteEventAsync(message);
    }
}