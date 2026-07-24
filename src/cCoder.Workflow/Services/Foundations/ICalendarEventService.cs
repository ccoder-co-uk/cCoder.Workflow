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
    ValueTask<CalendarEvent> AddAsync(CalendarEvent calendarEvent);
    ValueTask<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent);
    ValueTask DeleteAsync(int calendarEventId);
    ValueTask DeleteAllForAppAsync(IEnumerable<CalendarEvent> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}