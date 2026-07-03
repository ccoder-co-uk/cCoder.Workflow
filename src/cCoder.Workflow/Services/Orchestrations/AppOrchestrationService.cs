using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

internal class AppOrchestrationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    ICalendarOrchestrationService calendarOrchestrationService,
    IScheduledTaskOrchestrationService scheduledTaskOrchestrationService)
    : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampCalendars(app);
        StampFlows(app);
        StampScheduledTasks(app);
        _ = await calendarOrchestrationService.AddOrUpdate(app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdate(app.Tasks ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampCalendars(app);
        StampFlows(app);
        StampScheduledTasks(app);
        _ = await calendarOrchestrationService.AddOrUpdate(app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdate(app.Tasks ?? []);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await scheduledTaskOrchestrationService.DeleteByAppIdAsync(appId);
        await calendarOrchestrationService.DeleteByAppIdAsync(appId);
        await flowDefinitionOrchestrationService.DeleteByAppIdAsync(appId);
    }

    private static void StampCalendars(App app)
    {
        foreach (Calendar calendar in app.Calendars ?? [])
            calendar.AppId = app.Id;
    }

    private static void StampFlows(App app)
    {
        foreach (FlowDefinition flow in app.Flows ?? [])
            flow.AppId = app.Id;
    }

    private static void StampScheduledTasks(App app)
    {
        foreach (ScheduledTask task in app.Tasks ?? [])
            task.AppId = app.Id;
    }
}

