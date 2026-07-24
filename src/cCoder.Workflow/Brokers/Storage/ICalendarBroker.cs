// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarBroker
{
    IQueryable<Calendar> GetAllCalendars(bool ignoreFilters);

    ValueTask<Calendar> AddCalendarAsync(Calendar entity);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar entity);

    ValueTask<int> DeleteCalendarAsync(Calendar entity);

    ValueTask DeleteAllCalendarsAsync(IEnumerable<Calendar> items);

    ValueTask DeleteAllCalendarsByAppIdAsync(int appId);

    int? GetAppId(Calendar entity);
}