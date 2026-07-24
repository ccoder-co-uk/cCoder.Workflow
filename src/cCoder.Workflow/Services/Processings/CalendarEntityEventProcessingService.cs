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
    public ValueTask RaiseCalendarAddEventAsync(Calendar entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarAddEventAsync(Calendar entity) =>
        eventService.RaiseCalendarAddEventAsync(entity: entity);

    public ValueTask RaiseCalendarUpdateEventAsync(Calendar entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarUpdateEventAsync(Calendar entity) =>
        eventService.RaiseCalendarUpdateEventAsync(entity: entity);

    public ValueTask RaiseCalendarDeleteEventAsync(Calendar entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseCalendarDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseCalendarDeleteEventAsync(Calendar entity) =>
        eventService.RaiseCalendarDeleteEventAsync(entity: entity);
}