// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarEventBroker
{
    IQueryable<CalendarEvent> SelectAllCalendarEvents();

    IQueryable<CalendarEvent> SelectAllCalendarEventsIgnoringQueryFilters();

    ValueTask<CalendarEvent> InsertCalendarEventAsync(CalendarEvent newCalendarEvent);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent);

    ValueTask<int> DeleteCalendarEventAsync(CalendarEvent deletedCalendarEvent);

    ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> deletedItems);

    ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId);

    int? SelectAppId(CalendarEvent calendarEvent);
}