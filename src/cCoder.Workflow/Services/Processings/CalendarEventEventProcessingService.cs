// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class CalendarEventEventProcessingService(ICalendarEventEventService eventService) : ICalendarEventEventProcessingService
{
    public ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent entity) => eventService.RaiseCalendarEventAddEventAsync(entity:entity);

    public ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent entity) => eventService.RaiseCalendarEventUpdateEventAsync(entity:entity);

    public ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent entity) => eventService.RaiseCalendarEventDeleteEventAsync(entity:entity);
}