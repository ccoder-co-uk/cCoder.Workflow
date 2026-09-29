// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface ICalendarEventEventService
{
    ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent calendarEvent);

    ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent calendarEvent);

    ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent calendarEvent);
}