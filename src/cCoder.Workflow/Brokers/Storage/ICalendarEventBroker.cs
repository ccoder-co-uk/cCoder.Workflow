using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarEventBroker
{
    IQueryable<CalendarEvent> GetAllCalendarEvents(bool ignoreFilters);
    ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent entity);
    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent entity);
    ValueTask<int> DeleteCalendarEventAsync(CalendarEvent entity);
    ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> items);
    ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId);
    int? GetAppId(CalendarEvent entity);
}







