// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface ICalendarEventEventService
{
    ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent calendarEvent);

    ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent calendarEvent);

    ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent calendarEvent);
}