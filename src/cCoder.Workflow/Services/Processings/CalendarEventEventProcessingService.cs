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
    public ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarEventAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventAddEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventAddEventAsync(entity: entity);

    public ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarEventUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventUpdateEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventUpdateEventAsync(entity: entity);

    public ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarEventDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarEventDeleteEventAsync(CalendarEvent entity) =>
        eventService.RaiseCalendarEventDeleteEventAsync(entity: entity);
}