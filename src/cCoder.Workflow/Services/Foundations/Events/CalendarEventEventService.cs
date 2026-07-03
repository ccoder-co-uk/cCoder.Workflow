using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class CalendarEventEventService(
    ICalendarEventEventBroker calendarEventEventBroker,
    ICoreAuthInfo authInfo
) : ICalendarEventEventService
{
    public async ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent entity)
    {
        EventMessage<CalendarEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventEventBroker.RaiseCalendarEventAddEventAsync(message);
    }

    public async ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent entity)
    {
        EventMessage<CalendarEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventEventBroker.RaiseCalendarEventUpdateEventAsync(message);
    }

    public async ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent entity)
    {
        EventMessage<CalendarEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await calendarEventEventBroker.RaiseCalendarEventDeleteEventAsync(message);
    }
}
