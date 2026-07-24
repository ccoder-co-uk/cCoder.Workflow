// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Extensions;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Orchestrations;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;


namespace cCoder.Workflow.Services.Aggregations;

internal sealed partial class WorkflowMigrationAggregationService(
    ICalendarOrchestrationService calendarOrchestrationService,
    ICalendarEventOrchestrationService calendarEventOrchestrationService,
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    ILogger<WorkflowMigrationAggregationService> logger,
    IJsonBroker jsonBroker
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
            ? [jsonBroker.ParseJson<Calendar>(json: item.Data)]
            : jsonBroker.ParseJson<Calendar[]>(json: item.Data);

        string[] names = calendars.Select(selector: calendar => calendar.Name.ToLower())
            .ToArray();

        var existingCalendars = calendarOrchestrationService
            .GetAll()
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

        _ = await calendarOrchestrationService.AddOrUpdateCalendar(items: calendars.Where(predicate: calendar => calendar.Id == 0));
    }

    private async ValueTask ImportCalendarEventsAsync(int appId, WorkflowPackageItem item)
    {
        ImportCalendarEventInfo[] importSet = item.Data.StartsWith(value: "{")
            ? [jsonBroker.ParseJson<ImportCalendarEventInfo>(json: item.Data)]
            : jsonBroker.ParseJson<ImportCalendarEventInfo[]>(json: item.Data);

        Calendar[] calendars = calendarOrchestrationService
            .GetAll(ignoreFilters: true)
            .Where(predicate: calendar => calendar.AppId == appId)
            .ToArray();

        string[] calendarEventNames = importSet.Select(selector: calendarEvent => calendarEvent.Name)
            .ToArray();

        CalendarEvent[] existingCalendarEvents = calendarEventOrchestrationService
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

        logger.LogDebug(
            message: "Importing {CalendarEventCount} new calendar events for app {AppId}",
            args: [calendarEventsToAdd.Count, appId]);

        _ = await calendarEventOrchestrationService.AddOrUpdateCalendarEvent(items: [.. calendarEventsToAdd]);
    }

    private async ValueTask ImportFlowDefinitionsAsync(int appId, WorkflowPackageItem item)
    {
        FlowDefinition[] flowDefinitions = item.Data.StartsWith(value: "{")
            ? [jsonBroker.ParseJson<FlowDefinition>(json: item.Data)]
            : jsonBroker.ParseJson<FlowDefinition[]>(json: item.Data);

        string[] names = flowDefinitions.Select(selector: flowDefinition => flowDefinition.Name.ToLower())
            .ToArray();

        var existingFlowDefinitions = flowDefinitionOrchestrationService
            .GetAll()
            .Where(predicate: flowDefinition =>
                flowDefinition.AppId == appId && names.Contains(value: flowDefinition.Name.ToLower()))
            .Select(selector: flowDefinition => new
            {
                flowDefinition.Id,
                flowDefinition.Name
            })
            .ToArray();

        logger.LogDebug(
message: "Existing Flow Definition Items:\n{ExistingFlowDefinitions}",
args: cCoder.Workflow.Api.OData.ODataJsonExtensions.ToJsonForOdata(value: existingFlowDefinitions));

        for (int index = 0; index < flowDefinitions.Length; index++)
        {
            FlowDefinition flowDefinition = flowDefinitions[index];
            var existingFlowDefinition = existingFlowDefinitions.FirstOrDefault(predicate: existing => existing.Name.Equals(value: flowDefinition.Name, comparisonType: StringComparison.OrdinalIgnoreCase));
            flowDefinition.AppId = appId;
            flowDefinition.Id = existingFlowDefinition?.Id ?? Guid.Empty;
        }

        _ = await flowDefinitionOrchestrationService.AddOrUpdateFlowDefinition(items: flowDefinitions);
    }

    private cCoder.Data.Models.Packaging.Package ExportCalendars(int appId) =>
        new("Calendars")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/Calendar",
                    Data = calendarOrchestrationService
                        .GetAll(ignoreFilters:false)
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
                    Data = calendarEventOrchestrationService
                        .GetAll(ignoreFilters:false)
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
                    Data = jsonBroker.Serialize(
value:                        flowDefinitionOrchestrationService
                            .GetAll(ignoreFilters:false)
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
}