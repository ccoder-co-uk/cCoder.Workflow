// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        StampCalendars(app:app);
        StampFlows(app:app);
        StampScheduledTasks(app:app);
        _ = await calendarOrchestrationService.AddOrUpdate(items:app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(items:app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdate(items:app.Tasks ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampCalendars(app:app);
        StampFlows(app:app);
        StampScheduledTasks(app:app);
        _ = await calendarOrchestrationService.AddOrUpdate(items:app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdate(items:app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdate(items:app.Tasks ?? []);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await scheduledTaskOrchestrationService.DeleteByAppIdAsync(appId:appId);
        await calendarOrchestrationService.DeleteByAppIdAsync(appId:appId);
        await flowDefinitionOrchestrationService.DeleteByAppIdAsync(appId:appId);
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