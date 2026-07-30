// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Extensions;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;
using cCoder.Workflow.Models;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models.Results;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Brokers;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;


namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class WorkflowMigrationAggregationService(
    IWorkflowMigrationServiceProviderBroker serviceProviderBroker
) : IWorkflowMigrationAggregationService
{
    public ValueTask ImportPackageWorkflowPackageAsync(int appId, WorkflowPackage package) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId, package]); await ExecuteImportPackageAsync(appId: appId, package: package); }, isValueTask: true);

    private async ValueTask ExecuteImportPackageAsync(int appId, WorkflowPackage package)
    {
        if (package.Items is null || package.Items.Count == 0)
        {
            return;
        }

        foreach (WorkflowPackageItem item in package.Items)
        {
            switch (item.Type)
            {
                case "Core/Calendar":
                    await ImportCalendarsAsync(appId: appId, item: item);
                    break;
                case "Core/CalendarEvent":
                    await ImportCalendarEventsAsync(appId: appId, item: item);
                    break;
                case "Core/FlowDefinition":
                    await ImportFlowDefinitionsAsync(appId: appId, item: item);
                    break;
                case "Core/ScheduledTask":
                    await ImportScheduledTasksAsync(appId: appId, item: item);
                    break;
            }
        }
    }

    public WorkflowPackage ExportPackage(int appId, string packageName) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [appId, packageName]); return ExecuteExportPackage(appId: appId, packageName: packageName); });

    private WorkflowPackage ExecuteExportPackage(int appId, string packageName)
    {
        var package = packageName switch
        {
            "Calendars" => ExportCalendars(appId: appId),
            "CalendarEvents" => ExportCalendarEvents(appId: appId),
            "Workflows" => ExportFlowDefinitions(appId: appId),
            "ScheduledTasks" => ExportScheduledTasks(appId: appId),
            _ => new Data.Models.Packaging.Package(packageName) { Items = [] },
        };

        return new WorkflowPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?
                .Select(selector: item => new WorkflowPackageItem
                {
                    Id = item.Id,
                    PackageId = item.PackageId,
                    Type = item.Type,
                    Data = item.Data,
                })
                .ToArray(),
        };
    }

    private async ValueTask ImportCalendarsAsync(int appId, WorkflowPackageItem item)
    {
        Calendar[] calendars = item.Data.StartsWith(value: "{")
            ? [GetJsonBroker()
                .ParseJson<Calendar>(json: item.Data)]
            : GetJsonBroker()
                .ParseJson<Calendar[]>(json: item.Data);

        string[] names = calendars.Select(selector: calendar => calendar.Name.ToLower())
            .ToArray();

        var existingCalendars = GetCalendarOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendar => calendar.AppId == appId && names.Contains(value: calendar.Name.ToLower()))
            .Select(selector: calendar => new { calendar.Id, calendar.Name })
            .ToArray();

        Array.ForEach(
array: calendars,
action: calendar =>
            {
                calendar.AppId = appId;

                calendar.Id =
                    existingCalendars.FirstOrDefault(predicate: existing =>
                        existing.Name.Equals(
                            value: calendar.Name,
                            comparisonType: StringComparison.OrdinalIgnoreCase))
                    ?.Id ?? 0;
            });

        IEnumerable<Result<Calendar>> results = await GetCalendarOrchestrationService()
            .AddOrUpdateCalendar(items: calendars.Where(predicate: calendar => calendar.Id == 0));

        EnsureImportSucceeded(itemType: "calendars", results: results);
    }

    private async ValueTask ImportCalendarEventsAsync(int appId, WorkflowPackageItem item)
    {
        ImportCalendarEventInfo[] importSet = item.Data.StartsWith(value: "{")
            ? [GetJsonBroker()
                .ParseJson<ImportCalendarEventInfo>(json: item.Data)]
            : GetJsonBroker()
                .ParseJson<ImportCalendarEventInfo[]>(json: item.Data);

        Calendar[] calendars = GetCalendarOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendar => calendar.AppId == appId)
            .ToArray();

        string[] calendarEventNames = importSet.Select(selector: calendarEvent => calendarEvent.Name)
            .ToArray();

        CalendarEvent[] existingCalendarEvents = GetCalendarEventOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendarEvent =>
                calendarEvent.Calendar.AppId == appId && calendarEventNames.Contains(value: calendarEvent.Name))
            .ToArray();

        List<CalendarEvent> calendarEventsToAdd = [];

        foreach (ImportCalendarEventInfo importInfo in importSet)
        {
            CalendarEvent calendarEvent = new()
            {
                Id =
                    existingCalendarEvents.FirstOrDefault(predicate: existing =>
                        existing.Name == importInfo.Name
                        && existing.Calendar.Name == importInfo.CalendarName)
                    ?.Id ?? 0,
                CalendarId =
                    calendars.FirstOrDefault(predicate: calendar => calendar.Name == importInfo.CalendarName)?.Id ?? 0,
                Name = importInfo.Name,
                DurationInTicks = importInfo.DurationInTicks,
                Start = importInfo.Start,
                Description = importInfo.Description,
            };

            if (calendarEvent.CalendarId == 0 || calendarEvent.Id != 0)
            {
                continue;
            }

            calendarEventsToAdd.Add(item: calendarEvent);
        }

        GetLogger()
            .LogDebug(
            message: "Importing {CalendarEventCount} new calendar events for app {AppId}",
            args: [calendarEventsToAdd.Count, appId]);

        IEnumerable<Result<CalendarEvent>> results = await GetCalendarEventOrchestrationService()
            .AddOrUpdateCalendarEvent(items: [.. calendarEventsToAdd]);

        EnsureImportSucceeded(itemType: "calendar events", results: results);
    }

    private async ValueTask ImportFlowDefinitionsAsync(int appId, WorkflowPackageItem item)
    {
        FlowDefinition[] flowDefinitions = item.Data.StartsWith(value: "{")
            ? [GetJsonBroker()
                .ParseJson<FlowDefinition>(json: item.Data)]
            : GetJsonBroker()
                .ParseJson<FlowDefinition[]>(json: item.Data);

        string[] names = flowDefinitions.Select(selector: flowDefinition => flowDefinition.Name.ToLower())
            .ToArray();

        var existingFlowDefinitions = GetFlowDefinitionOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: flowDefinition =>
                flowDefinition.AppId == appId && names.Contains(value: flowDefinition.Name.ToLower()))
            .Select(selector: flowDefinition => new
            {
                flowDefinition.Id,
                flowDefinition.Name
            })
            .ToArray();

        GetLogger()
            .LogDebug(
message: "Existing Flow Definition Items:\n{ExistingFlowDefinitions}",
args: cCoder.Workflow.Extensions.OData.ObjectExtensions.ToJsonForOdata(value: existingFlowDefinitions));

        for (int index = 0; index < flowDefinitions.Length; index++)
        {
            FlowDefinition flowDefinition = flowDefinitions[index];
            var existingFlowDefinition = existingFlowDefinitions.FirstOrDefault(predicate: existing => existing.Name.Equals(value: flowDefinition.Name, comparisonType: StringComparison.OrdinalIgnoreCase));
            flowDefinition.AppId = appId;
            flowDefinition.Id = existingFlowDefinition?.Id ?? Guid.Empty;
        }

        IEnumerable<Result<FlowDefinition>> results = await GetFlowDefinitionOrchestrationService()
            .AddOrUpdateFlowDefinition(items: flowDefinitions);

        EnsureImportSucceeded(itemType: "flow definitions", results: results);
    }

    private async ValueTask ImportScheduledTasksAsync(
        int appId,
        WorkflowPackageItem item)
    {
        ImportScheduledTaskInfo[] importSet = item.Data.StartsWith(value: "{")
            ? [GetJsonBroker().ParseJson<ImportScheduledTaskInfo>(json: item.Data)]
            : GetJsonBroker().ParseJson<ImportScheduledTaskInfo[]>(json: item.Data);

        FlowDefinition[] flows = GetFlowDefinitionOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: flow => flow.AppId == appId)
            .ToArray();

        ScheduledTask[] existingTasks = GetScheduledTaskOrchestrationService()
            .GetAll(ignoreFilters: true)
            .Where(predicate: task => task.AppId == appId)
            .ToArray();

        List<ScheduledTask> tasks = [];

        foreach (ImportScheduledTaskInfo importInfo in importSet)
        {
            FlowDefinition flow = flows.FirstOrDefault(predicate: candidate =>
                candidate.Name.Equals(
                    value: importInfo.FlowName,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            if (flow is null)
            {
                throw new InvalidOperationException(
                    message:
                        $"Cannot import scheduled task '{importInfo.Name}' because flow '{importInfo.FlowName}' does not exist in app {appId}.");
            }

            ScheduledTask existingTask = existingTasks.FirstOrDefault(
                predicate: candidate => candidate.Name.Equals(
                    value: importInfo.Name,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            ScheduledTask task = existingTask ?? new ScheduledTask();
            task.AppId = appId;
            task.FlowId = flow.Id;
            task.Name = importInfo.Name;
            task.Description = importInfo.Description;
            task.ExecuteAs =
                string.IsNullOrWhiteSpace(value: importInfo.ExecuteAs)
                    ? GetAuthorizationBroker().GetCurrentUser().Id
                    : importInfo.ExecuteAs;
            task.ExecutionArgs = importInfo.ExecutionArgs;
            task.ScheduleInTicks = importInfo.ScheduleInTicks;
            task.NextExecution =
                importInfo.NextExecution
                ?? existingTask?.NextExecution
                ?? (importInfo.ScheduleInTicks > 0
                    ? DateTimeOffset.UtcNow.AddTicks(
                        ticks: importInfo.ScheduleInTicks)
                    : null);

            tasks.Add(item: task);
        }

        IEnumerable<Result<ScheduledTask>> results =
            await GetScheduledTaskOrchestrationService()
                .AddOrUpdateScheduledTask(items: tasks);

        EnsureImportSucceeded(itemType: "scheduled tasks", results: results);
    }

    private static void EnsureImportSucceeded<T>(
        string itemType,
        IEnumerable<Result<T>> results)
    {
        string[] failures = results
            .Where(predicate: result => !result.Success)
            .Select(selector: result =>
                string.IsNullOrWhiteSpace(value: result.Message)
                    ? result.Id ?? "Unknown item"
                    : result.Message)
            .ToArray();

        if (failures.Length > 0)
        {
            throw new InvalidOperationException(
                message: $"Failed to import {itemType}: {string.Join(separator: "; ", value: failures)}");
        }
    }

    private cCoder.Data.Models.Packaging.Package ExportCalendars(int appId) =>
        new("Calendars")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/Calendar",
                    Data = GetCalendarOrchestrationService()
                        .GetAll(ignoreFilters:true)
                        .Where(predicate:calendar => calendar.AppId == appId)
                        .Select(selector:calendar => new { calendar.Name, calendar.Description })
                        .ToArray()
                        .ToJson(),
                },
            ],
        };

    private cCoder.Data.Models.Packaging.Package ExportCalendarEvents(int appId) =>
        new("CalendarEvents")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/CalendarEvent",
                    Data = GetCalendarEventOrchestrationService()
                        .GetAll(ignoreFilters:true)
                        .ToArray()
                        .Where(predicate:calendarEvent => calendarEvent.Calendar != null && calendarEvent.Calendar.AppId == appId)
                        .Select(selector:calendarEvent => new
                        {
                            CalendarName = calendarEvent.Calendar.Name,
                            calendarEvent.Name,
                            calendarEvent.Start,
                            calendarEvent.Description,
                            calendarEvent.DurationInTicks,
                        })
                        .ToArray()
                        .ToJson(),
                },
            ],
        };

    private cCoder.Data.Models.Packaging.Package ExportFlowDefinitions(int appId) =>
        new("Workflows")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/FlowDefinition",
                    Data = GetJsonBroker()
                        .Serialize(
                            value: GetFlowDefinitionOrchestrationService()
                            .GetAll(ignoreFilters:true)
                            .Where(predicate:flowDefinition => flowDefinition.AppId == appId)
                            .Select(selector:flowDefinition => new
                            {
                                ProcessName = flowDefinition.App.Name,
                                flowDefinition.Name,
                                flowDefinition.ReportingComponentName,
                                flowDefinition.InstanceReportingComponentName,
                                flowDefinition.Description,
                                flowDefinition.DefinitionJson,
                                flowDefinition.ConfigJson,
                                flowDefinition.LastUpdated,
                            })
                            .ToArray()
                    ),
                },
            ],
        };

    private cCoder.Data.Models.Packaging.Package ExportScheduledTasks(int appId) =>
        new("ScheduledTasks")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/ScheduledTask",
                    Data = GetJsonBroker()
                        .Serialize(
                            value: GetScheduledTaskOrchestrationService()
                                .GetAll(ignoreFilters: true)
                                .Where(predicate: task => task.AppId == appId)
                                .Select(selector: task => new
                                {
                                    task.Name,
                                    task.Description,
                                    FlowName = task.Flow.Name,
                                    task.ExecuteAs,
                                    task.ExecutionArgs,
                                    task.ScheduleInTicks,
                                })
                                .ToArray()),
                },
            ],
        };

    private IAuthorizationBroker GetAuthorizationBroker() =>
        serviceProviderBroker.GetOperationService<IAuthorizationBroker>(
            operation: WorkflowMigrationOperation.Authorization);

    private ICalendarOrchestrationService GetCalendarOrchestrationService() =>
        serviceProviderBroker.GetOperationService<ICalendarOrchestrationService>(
            operation: WorkflowMigrationOperation.Calendar);

    private ICalendarEventOrchestrationService GetCalendarEventOrchestrationService() =>
        serviceProviderBroker.GetOperationService<ICalendarEventOrchestrationService>(
            operation: WorkflowMigrationOperation.CalendarEvent);

    private IFlowDefinitionOrchestrationService GetFlowDefinitionOrchestrationService() =>
        serviceProviderBroker.GetOperationService<IFlowDefinitionOrchestrationService>(
            operation: WorkflowMigrationOperation.FlowDefinition);

    private IScheduledTaskOrchestrationService GetScheduledTaskOrchestrationService() =>
        serviceProviderBroker.GetOperationService<IScheduledTaskOrchestrationService>(
            operation: WorkflowMigrationOperation.ScheduledTask);

    private IJsonBroker GetJsonBroker() =>
        serviceProviderBroker.GetOperationService<IJsonBroker>(
            operation: WorkflowMigrationOperation.Json);

    private ILogger<WorkflowMigrationAggregationService> GetLogger() =>
        serviceProviderBroker.GetOperationService<ILogger<WorkflowMigrationAggregationService>>(
            operation: WorkflowMigrationOperation.Logging);
}
