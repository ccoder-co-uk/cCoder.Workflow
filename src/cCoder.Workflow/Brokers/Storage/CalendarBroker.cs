// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Planning;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

internal sealed class CalendarBroker(ICoreContextFactory coreContextFactory) : ICalendarBroker
{

    public IQueryable<Calendar> SelectAllCalendars() =>
        coreContextFactory.CreateCoreContext().Calendars;

    public IQueryable<Calendar> SelectAllCalendarsIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .Calendars
            .IgnoreQueryFilters();

    public async ValueTask<Calendar> InsertCalendarAsync(Calendar entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Calendar result = (await coreDataContext.Calendars.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Calendar> UpdateCalendarAsync(Calendar entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Calendar result = coreDataContext.Calendars.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCalendarAsync(Calendar entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Calendars.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarsAsync(IEnumerable<Calendar> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Calendars.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCalendarsByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.Calendars
            .IgnoreQueryFilters()
            .Where(predicate: calendar => calendar.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? SelectAppId(Calendar entity)
    {
        return entity.AppId;
    }
}