// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface ICalendarEventService
{
    CalendarEvent Get(int calendarEventId);

    IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false);

    ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newCalendarEvent);

    ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent);

    ValueTask DeleteAsync(int calendarEventId);

    ValueTask DeleteAllForAppCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems);

    ValueTask DeleteAllByAppIdAsync(int appId);
}