// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarBroker
{
    IQueryable<Calendar> SelectAllCalendars();

    IQueryable<Calendar> SelectAllCalendarsIgnoringQueryFilters();

    ValueTask<Calendar> InsertCalendarAsync(Calendar newCalendar);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedCalendar);

    ValueTask<int> DeleteCalendarAsync(Calendar deletedCalendar);

    ValueTask DeleteAllCalendarsAsync(IEnumerable<Calendar> deletedItems);

    ValueTask DeleteAllCalendarsByAppIdAsync(int appId);

    int? SelectAppId(Calendar calendar);
}