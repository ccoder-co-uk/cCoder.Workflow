// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarBroker
{
    IQueryable<Calendar> SelectAllCalendars();

    IQueryable<Calendar> SelectAllCalendarsIgnoringQueryFilters();

    ValueTask<Calendar> InsertCalendarAsync(Calendar newEntity);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedEntity);

    ValueTask<int> DeleteCalendarAsync(Calendar deletedEntity);

    ValueTask DeleteAllCalendarsAsync(IEnumerable<Calendar> deletedItems);

    ValueTask DeleteAllCalendarsByAppIdAsync(int appId);

    int? SelectAppId(Calendar entity);
}