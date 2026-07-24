// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;


namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class CalendarCoordinationService(
    ICalendarEventOrchestrationService calendarEventOrchestrationService
) : ICalendarCoordinationService
{
    public ValueTask HandleCalendarAddAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteHandleCalendarAddAsync(calendar: calendar); }, isValueTask: true);

    private async ValueTask ExecuteHandleCalendarAddAsync(Calendar calendar)
    {
        if (calendar.Events == null || !calendar.Events.Any())
        {
            return;
        }

        await calendarEventOrchestrationService.AddOrUpdate(items: calendar.Events);
    }

    public ValueTask HandleCalendarUpdateAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteHandleCalendarUpdateAsync(calendar: calendar); }, isValueTask: true);

    private async ValueTask ExecuteHandleCalendarUpdateAsync(Calendar calendar)
    {
        if (calendar.Events == null || !calendar.Events.Any())
        {
            return;
        }

        await calendarEventOrchestrationService.AddOrUpdate(items: calendar.Events);
    }

    public ValueTask HandleCalendarDeleteAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); await ExecuteHandleCalendarDeleteAsync(calendar: calendar); }, isValueTask: true);

    private async ValueTask ExecuteHandleCalendarDeleteAsync(Calendar calendar)
    {
        IEnumerable<CalendarEvent> eventsToDelete = calendarEventOrchestrationService
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendarEvent => calendarEvent.CalendarId == calendar.Id)
            .ToArray();

        await calendarEventOrchestrationService.DeleteAllAsync(items: eventsToDelete);
    }
}