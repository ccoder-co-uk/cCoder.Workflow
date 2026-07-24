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

internal class CalendarCoordinationService(
    ICalendarEventOrchestrationService calendarEventOrchestrationService
) : ICalendarCoordinationService
{
    public async ValueTask HandleCalendarAddAsync(Calendar calendar)
    {
        if (calendar.Events == null || !calendar.Events.Any())
        {
            return;
        }

        await calendarEventOrchestrationService.AddOrUpdate(items: calendar.Events);
    }

    public async ValueTask HandleCalendarUpdateAsync(Calendar calendar)
    {
        if (calendar.Events == null || !calendar.Events.Any())
        {
            return;
        }

        await calendarEventOrchestrationService.AddOrUpdate(items: calendar.Events);
    }

    public async ValueTask HandleCalendarDeleteAsync(Calendar calendar)
    {
        IEnumerable<CalendarEvent> eventsToDelete = calendarEventOrchestrationService
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendarEvent => calendarEvent.CalendarId == calendar.Id)
            .ToArray();

        await calendarEventOrchestrationService.DeleteAllAsync(items: eventsToDelete);
    }
}