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

internal sealed partial class CalendarEventEventProcessingService(ICalendarEventEventService eventService) : ICalendarEventEventProcessingService
{
    public ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEvent]); await ExecuteRaiseCalendarEventAddEventAsync(entity: calendarEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventAddEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventAddEventAsync(calendarEvent: entity);

    public ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEvent]); await ExecuteRaiseCalendarEventUpdateEventAsync(entity: calendarEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventUpdateEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventUpdateEventAsync(calendarEvent: entity);

    public ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEvent]); await ExecuteRaiseCalendarEventDeleteEventAsync(entity: calendarEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventDeleteEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventDeleteEventAsync(calendarEvent: entity);
}