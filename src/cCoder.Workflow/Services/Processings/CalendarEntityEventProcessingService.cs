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

internal sealed partial class CalendarEntityEventProcessingService(ICalendarEntityEventService eventService) : ICalendarEntityEventProcessingService
{
    public ValueTask RaiseCalendarAddEventAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteRaiseCalendarAddEventAsync(entity: calendar); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarAddEventAsync(Calendar entity) =>
        eventService.RaiseCalendarAddEventAsync(calendar: entity);

    public ValueTask RaiseCalendarUpdateEventAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteRaiseCalendarUpdateEventAsync(entity: calendar); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarUpdateEventAsync(Calendar entity) =>
        eventService.RaiseCalendarUpdateEventAsync(calendar: entity);

    public ValueTask RaiseCalendarDeleteEventAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteRaiseCalendarDeleteEventAsync(entity: calendar); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarDeleteEventAsync(Calendar entity) =>
        eventService.RaiseCalendarDeleteEventAsync(calendar: entity);
}