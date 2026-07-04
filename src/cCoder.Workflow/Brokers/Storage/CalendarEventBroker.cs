using cCoder.Data;
using cCoder.Data.Models.Planning;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

public class CalendarEventBroker(ICoreContextFactory coreContextFactory) : ICalendarEventBroker
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
        CalendarEvent result = (await coreDataContext.Events.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CalendarEvent result = coreDataContext.Events.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCalendarEventAsync(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarEventsAsync(IEnumerable<CalendarEvent> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Events.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarEventsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.Events
            .IgnoreQueryFilters()
            .Where(calendarEvent => calendarEvent.Calendar.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(CalendarEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Calendars

            .Where(calendar => calendar.Id == entity.CalendarId)
            .Select(calendar => (int?)calendar.AppId)
            .FirstOrDefault();

    }
}







