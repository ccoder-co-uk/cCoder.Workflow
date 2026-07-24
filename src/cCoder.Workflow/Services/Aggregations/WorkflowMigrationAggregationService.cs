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

internal class WorkflowMigrationAggregationService(
    ICalendarOrchestrationService calendarOrchestrationService,
    ICalendarEventOrchestrationService calendarEventOrchestrationService,
    IFlowDefinitionOrchestrationService flowDefinitionOrchestrationService,
    ILogger<WorkflowMigrationAggregationService> logger,
    IJsonBroker jsonBroker
) : IWorkflowMigrationAggregationService
{
    public async ValueTask ImportPackageAsync(int appId, WorkflowPackage package)
    {
        if (package.Items is null || package.Items.Count == 0)
            return;

        foreach (WorkflowPackageItem item in package.Items)
        {
            switch (item.Type)
            {
                case "Core/Calendar":
                    await ImportCalendarsAsync(appId, item);
                    break;
                case "Core/CalendarEvent":
                    await ImportCalendarEventsAsync(appId, item);
                    break;
                case "Core/FlowDefinition":
                    await ImportFlowDefinitionsAsync(appId, item);
                    break;
            }
        }
    }

    public WorkflowPackage ExportPackage(int appId, string packageName)
    {
        var package = packageName switch
        {
            "Calendars" => ExportCalendars(appId),
            "CalendarEvents" => ExportCalendarEvents(appId),
            "Workflows" => ExportFlowDefinitions(appId),
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
                .Select(item => new WorkflowPackageItem
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
        Calendar[] calendars = item.Data.StartsWith("{")
            ? [jsonBroker.ParseJson<Calendar>(item.Data)]
            : jsonBroker.ParseJson<Calendar[]>(item.Data);

        string[] names = calendars.Select(calendar => calendar.Name.ToLower()).ToArray();

        var existingCalendars = calendarOrchestrationService
            .GetAll()
            .Where(calendar => calendar.AppId == appId && names.Contains(calendar.Name.ToLower()))
            .Select(calendar => new { calendar.Id, calendar.Name })
            .ToArray();

        Array.ForEach(
            calendars,
            calendar =>
            {
                calendar.AppId = appId;
                calendar.Id =
                    existingCalendars.FirstOrDefault(existing =>
                        existing.Name.Equals(calendar.Name, StringComparison.OrdinalIgnoreCase))
                    ?.Id ?? 0;
            });

        _ = await calendarOrchestrationService.AddOrUpdate(calendars.Where(calendar => calendar.Id == 0));
    }

    private async ValueTask ImportCalendarEventsAsync(int appId, WorkflowPackageItem item)
    {
        ImportCalendarEventInfo[] importSet = item.Data.StartsWith("{")
            ? [jsonBroker.ParseJson<ImportCalendarEventInfo>(item.Data)]
            : jsonBroker.ParseJson<ImportCalendarEventInfo[]>(item.Data);

        Calendar[] calendars = calendarOrchestrationService
            .GetAll(true)
            .Where(calendar => calendar.AppId == appId)
            .ToArray();

        string[] calendarEventNames = importSet.Select(calendarEvent => calendarEvent.Name).ToArray();

        CalendarEvent[] existingCalendarEvents = calendarEventOrchestrationService
            .GetAll(true)
            .Where(calendarEvent =>
                calendarEvent.Calendar.AppId == appId && calendarEventNames.Contains(calendarEvent.Name))
            .ToArray();

        List<CalendarEvent> calendarEventsToAdd = [];

        foreach (ImportCalendarEventInfo importInfo in importSet)
        {
            CalendarEvent calendarEvent = new()
            {
                Id =
                    existingCalendarEvents.FirstOrDefault(existing =>
                        existing.Name == importInfo.Name
                        && existing.Calendar.Name == importInfo.CalendarName)
                    ?.Id ?? 0,
                CalendarId =
                    calendars.FirstOrDefault(calendar => calendar.Name == importInfo.CalendarName)?.Id ?? 0,
                Name = importInfo.Name,
                DurationInTicks = importInfo.DurationInTicks,
                Start = importInfo.Start,
                Description = importInfo.Description,
            };

            if (calendarEvent.CalendarId == 0 || calendarEvent.Id != 0)
                continue;

            calendarEventsToAdd.Add(calendarEvent);
        }

        logger.LogDebug(
            "Importing {CalendarEventCount} new calendar events for app {AppId}",
            calendarEventsToAdd.Count,
            appId);

        _ = await calendarEventOrchestrationService.AddOrUpdate([.. calendarEventsToAdd]);
    }

    private async ValueTask ImportFlowDefinitionsAsync(int appId, WorkflowPackageItem item)
    {
        FlowDefinition[] flowDefinitions = item.Data.StartsWith("{")
            ? [jsonBroker.ParseJson<FlowDefinition>(item.Data)]
            : jsonBroker.ParseJson<FlowDefinition[]>(item.Data);

        string[] names = flowDefinitions.Select(flowDefinition => flowDefinition.Name.ToLower()).ToArray();

        var existingFlowDefinitions = flowDefinitionOrchestrationService
            .GetAll()
            .Where(flowDefinition =>
                flowDefinition.AppId == appId && names.Contains(flowDefinition.Name.ToLower()))
            .Select(flowDefinition => new
            {
                flowDefinition.Id,
                flowDefinition.Name
            })
            .ToArray();

        logger.LogDebug(
            "Existing Flow Definition Items:\n{ExistingFlowDefinitions}",
            cCoder.Workflow.Api.OData.ODataJsonExtensions.ToJsonForOdata(existingFlowDefinitions));

        for (int index = 0; index < flowDefinitions.Length; index++)
        {
            FlowDefinition flowDefinition = flowDefinitions[index];
            var existingFlowDefinition = existingFlowDefinitions.FirstOrDefault(existing => existing.Name.Equals(flowDefinition.Name, StringComparison.OrdinalIgnoreCase));
            flowDefinition.AppId = appId;
            flowDefinition.Id = existingFlowDefinition?.Id ?? Guid.Empty;
        }

        _ = await flowDefinitionOrchestrationService.AddOrUpdate(flowDefinitions);
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
                        .GetAll(false)
                        .Where(calendar => calendar.AppId == appId)
                        .Select(calendar => new { calendar.Name, calendar.Description })
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
                        .GetAll(false)
                        .ToArray()
                        .Where(calendarEvent => calendarEvent.Calendar != null && calendarEvent.Calendar.AppId == appId)
                        .Select(calendarEvent => new
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
                        flowDefinitionOrchestrationService
                            .GetAll(false)
                            .Where(flowDefinition => flowDefinition.AppId == appId)
                            .Select(flowDefinition => new
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