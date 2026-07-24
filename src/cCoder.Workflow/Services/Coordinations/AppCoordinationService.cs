// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Services.Coordinations;

internal sealed partial class AppCoordinationService(
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    ICalendarOrchestrationService calendarOrchestrationService,
    IScheduledTaskOrchestrationService scheduledTaskOrchestrationService)
    : IAppCoordinationService
{
    public ValueTask AddAppAsync(App newApp) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newApp]); await ExecuteAddAsync(app: newApp); }, isValueTask: true);

    private async ValueTask ExecuteAddAsync(App app)
    {
        StampCalendars(app: app);
        StampFlows(app: app);
        StampScheduledTasks(app: app);
        _ = await calendarOrchestrationService.AddOrUpdateCalendar(items: app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdateFlowDefinition(items: app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdateScheduledTask(items: app.Tasks ?? []);
    }

    public ValueTask UpdateAppAsync(App updatedApp) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedApp]); await ExecuteUpdateAsync(app: updatedApp); }, isValueTask: true);

    private async ValueTask ExecuteUpdateAsync(App app)
    {
        StampCalendars(app: app);
        StampFlows(app: app);
        StampScheduledTasks(app: app);
        _ = await calendarOrchestrationService.AddOrUpdateCalendar(items: app.Calendars ?? []);
        _ = await flowDefinitionOrchestrationService.AddOrUpdateFlowDefinition(items: app.Flows ?? []);
        _ = await scheduledTaskOrchestrationService.AddOrUpdateScheduledTask(items: app.Tasks ?? []);
    }

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteAsync(appId: appId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int appId)
    {
        await scheduledTaskOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await calendarOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await flowDefinitionOrchestrationService.DeleteByAppIdAsync(appId: appId);
    }

    private static void StampCalendars(App app)
    {
        foreach (Calendar calendar in app.Calendars ?? [])
        {
            calendar.AppId = app.Id;
        }
    }

    private static void StampFlows(App app)
    {
        foreach (FlowDefinition flow in app.Flows ?? [])
        {
            flow.AppId = app.Id;
        }
    }

    private static void StampScheduledTasks(App app)
    {
        foreach (ScheduledTask task in app.Tasks ?? [])
        {
            task.AppId = app.Id;
        }
    }
}
