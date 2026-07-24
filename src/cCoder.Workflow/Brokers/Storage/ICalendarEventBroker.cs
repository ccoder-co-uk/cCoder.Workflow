// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarEventBroker
{
    IQueryable<CalendarEvent> SelectAllCalendarEvents();

    IQueryable<CalendarEvent> SelectAllCalendarEventsIgnoringQueryFilters();

    ValueTask<CalendarEvent> InsertCalendarEventAsync(CalendarEvent newEntity);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedEntity);

    ValueTask<int> DeleteCalendarEventAsync(CalendarEvent deletedEntity);

    ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> deletedItems);

    ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId);

    int? SelectAppId(CalendarEvent entity);
}