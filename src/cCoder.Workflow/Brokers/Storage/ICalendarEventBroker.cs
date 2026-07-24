// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarEventBroker
{
    IQueryable<CalendarEvent> SelectAllCalendarEvents();

    IQueryable<CalendarEvent> SelectAllCalendarEventsIgnoringQueryFilters();

    ValueTask<CalendarEvent> InsertCalendarEventAsync(CalendarEvent entity);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent entity);

    ValueTask<int> DeleteCalendarEventAsync(CalendarEvent entity);

    ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> items);

    ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId);

    int? SelectAppId(CalendarEvent entity);
}