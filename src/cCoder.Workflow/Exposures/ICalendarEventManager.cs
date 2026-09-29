// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Workflow.Models.Results;
using cCoder.Data.Models.Planning;

namespace cCoder.Workflow.Exposures;

public interface ICalendarEventManager : IControllerErrorLogger
{
    CalendarEvent Get(int calendarEventId);

    IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false);

    ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newCalendarEvent);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent);

    ValueTask DeleteAsync(int calendarEventId);

    ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdateCalendarEvent(IEnumerable<CalendarEvent> items);

    ValueTask DeleteAllCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems);
}