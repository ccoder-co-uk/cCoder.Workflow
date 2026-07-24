// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface ICalendarEventOrchestrationService
{
    CalendarEvent Get(int calendarEventId);

    IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false);

    ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newEntity);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedEntity);

    ValueTask DeleteAsync(int calendarEventId);

    ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdateCalendarEvent(IEnumerable<CalendarEvent> items);

    ValueTask DeleteAllCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems);
}