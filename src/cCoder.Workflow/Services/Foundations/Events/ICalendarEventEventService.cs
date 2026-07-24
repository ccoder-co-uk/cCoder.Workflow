// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

public interface ICalendarEventEventService
{
    ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent entity);
    ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent entity);
    ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent entity);
}