// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Planning;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

internal sealed class CalendarEventBroker(ICoreContextFactory coreContextFactory) : ICalendarEventBroker
{

    public IQueryable<CalendarEvent> GetAllCalendarEvents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Events.IgnoreQueryFilters()
            : coreDataContext.Events;
    }

    public async ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CalendarEvent result = (await coreDataContext.Events.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CalendarEvent result = coreDataContext.Events.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCalendarEventAsync(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.RemoveRange(entities: items);
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

    public int? GetAppId(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Calendars

            .Where(predicate: calendar => calendar.Id == entity.CalendarId)
            .Select(selector: calendar => (int?)calendar.AppId)
            .FirstOrDefault();

    }
}