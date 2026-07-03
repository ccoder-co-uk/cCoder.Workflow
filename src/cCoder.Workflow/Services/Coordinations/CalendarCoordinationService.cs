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
            return;

        await calendarEventOrchestrationService.AddOrUpdate(calendar.Events);
    }

    public async ValueTask HandleCalendarUpdateAsync(Calendar calendar)
    {
        if (calendar.Events == null || !calendar.Events.Any())
            return;

        await calendarEventOrchestrationService.AddOrUpdate(calendar.Events);
    }

    public async ValueTask HandleCalendarDeleteAsync(Calendar calendar)
    {
        IEnumerable<CalendarEvent> eventsToDelete = calendarEventOrchestrationService
            .GetAll(true)
            .Where(calendarEvent => calendarEvent.CalendarId == calendar.Id)
            .ToArray();

        await calendarEventOrchestrationService.DeleteAllAsync(eventsToDelete);
    }
}











