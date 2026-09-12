// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Planning;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

internal sealed class CalendarEventBroker(ICoreContextFactory coreContextFactory) : ICalendarEventBroker
{

    public IQueryable<CalendarEvent> SelectAllCalendarEvents() =>
        coreContextFactory.CreateCoreContext().Events;

    public IQueryable<CalendarEvent> SelectAllCalendarEventsIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .Events
            .IgnoreQueryFilters();

    public async ValueTask<CalendarEvent> InsertCalendarEventAsync(CalendarEvent newCalendarEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CalendarEvent result = (await coreDataContext.Events.AddAsync(entity: newCalendarEvent)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CalendarEvent result = coreDataContext.Events.Update(entity: updatedCalendarEvent).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCalendarEventAsync(CalendarEvent deletedCalendarEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.Remove(entity: deletedCalendarEvent);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> deletedItems)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.RemoveRange(entities: deletedItems);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.Events
            .IgnoreQueryFilters()
            .Where(predicate: calendarEvent => calendarEvent.Calendar.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? SelectAppId(CalendarEvent calendarEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Calendars

            .Where(predicate: calendar => calendar.Id == calendarEvent.CalendarId)
            .Select(selector: calendar => (int?)calendar.AppId)
            .FirstOrDefault();

    }
}