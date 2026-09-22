// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Runtime.CompilerServices;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.Workflow;

public static partial class IEventHubExtensions
{
    private static readonly ConditionalWeakTable<IEventHub, object> ListeningEventHubs = new();
    private static readonly object ListeningEventHubsLock = new();

    public static IEventHub ListenToWorkflowEvents(this IEventHub eventHub)
    {
        ArgumentNullException.ThrowIfNull(argument: eventHub);

        lock (ListeningEventHubsLock)
        {
            if (ListeningEventHubs.TryGetValue(key: eventHub, value: out _))
            {
                return eventHub;
            }

            ListeningEventHubs.Add(key: eventHub, value: new object());
        }

        ListenToAppEvents(eventHub: eventHub);
        ListenToCalendarEvents(eventHub: eventHub);
        ListenToFlowDefinitionEvents(eventHub: eventHub);
        ListenToPackageEvents(eventHub: eventHub);
        ListenToWorkflowTriggerEvents(eventHub: eventHub);
        ListenToScheduledTaskExecuteEvents(eventHub: eventHub);
        ListenToQueuedFlowInstanceExecuteEvents(eventHub: eventHub);

        return eventHub;
    }

    private static void ListenToAppEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_add",
            handler: (IAppCoordinationService service, App app) =>
                service.AddAppAsync(newApp: app));

        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IAppCoordinationService service, App app) =>
                service.UpdateAppAsync(updatedApp: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppCoordinationService service, App app) =>
                service.DeleteAsync(appId: app.Id));
    }

    private static void ListenToCalendarEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "calendar_add",
            handler: async (ICalendarEventOrchestrationService service, Calendar calendar) =>
            {
                _ = await service.AddOrUpdateCalendarEvent(
                    items: calendar.Events ?? []);
            });

        eventHub.ListenToEvent(
            name: "calendar_update",
            handler: async (ICalendarEventOrchestrationService service, Calendar calendar) =>
            {
                _ = await service.AddOrUpdateCalendarEvent(
                    items: calendar.Events ?? []);
            });

        eventHub.ListenToEvent(
            name: "calendar_delete",
            handler: (ICalendarEventOrchestrationService service, Calendar calendar) =>
                service.DeleteAllCalendarEventAsync(
                    deletedItems: calendar.Events ?? []));
    }

    private static void ListenToFlowDefinitionEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "flow_definition_delete",
            handler: (IFlowDefinitionCoordinationService service, FlowDefinition flowDefinition) =>
                service.HandleFlowDefinitionDeleteAsync(
                    flowDefinition: flowDefinition));

    private static void ListenToPackageEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "package_import",
            handler: (IWorkflowMigrationAggregationService service,
                WorkflowPackageEvent packageEvent) =>
                service.ImportPackageWorkflowPackageAsync(
                    appId: packageEvent.AppId,
                    workflowPackage: ToLocalPackage(
                        package: packageEvent.Package)));

    private static void ListenToWorkflowTriggerEvents(IEventHub eventHub)
    {
        ListenToWorkflowTriggerEvents<App>(eventHub: eventHub, eventStem: "app");
        ListenToWorkflowTriggerEvents<AppCulture>(eventHub: eventHub, eventStem: "app_culture");
        ListenToWorkflowTriggerEvents<Calendar>(eventHub: eventHub, eventStem: "calendar");
        ListenToWorkflowTriggerEvents<CalendarEvent>(eventHub: eventHub, eventStem: "calendar_event");
        ListenToWorkflowTriggerEvents<CommonObject>(eventHub: eventHub, eventStem: "common_object");
        ListenToWorkflowTriggerEvents<Component>(eventHub: eventHub, eventStem: "component");
        ListenToWorkflowTriggerEvents<Content>(eventHub: eventHub, eventStem: "content");
        ListenToWorkflowTriggerEvents<Culture>(eventHub: eventHub, eventStem: "culture");
        ListenToWorkflowTriggerEvents<DataFile>(eventHub: eventHub, eventStem: "file");
        ListenToWorkflowTriggerEvents<FileContent>(eventHub: eventHub, eventStem: "file_content");
        ListenToWorkflowTriggerEvents<FlowDefinition>(eventHub: eventHub, eventStem: "flow_definition");
        ListenToWorkflowTriggerEvents<FlowInstanceData>(eventHub: eventHub, eventStem: "flow_instance_data");
        ListenToWorkflowTriggerEvents<Folder>(eventHub: eventHub, eventStem: "folder");
        ListenToWorkflowTriggerEvents<FolderRole>(eventHub: eventHub, eventStem: "folder_role");
        ListenToWorkflowTriggerEvents<Layout>(eventHub: eventHub, eventStem: "layout");
        ListenToWorkflowTriggerEvents<LogDataItem>(eventHub: eventHub, eventStem: "log_data_item");
        ListenToWorkflowTriggerEvents<LogEntry>(eventHub: eventHub, eventStem: "log_entry");
        ListenToWorkflowTriggerEvents<MailServer>(eventHub: eventHub, eventStem: "mail_server");
        ListenToWorkflowTriggerEvents<Package>(eventHub: eventHub, eventStem: "package");
        ListenToWorkflowTriggerEvents<DataPackageItem>(eventHub: eventHub, eventStem: "package_item");
        ListenToWorkflowTriggerEvents<Page>(eventHub: eventHub, eventStem: "page");
        ListenToWorkflowTriggerEvents<PageInfo>(eventHub: eventHub, eventStem: "page_info");
        ListenToWorkflowTriggerEvents<PageRole>(eventHub: eventHub, eventStem: "page_role");
        ListenToWorkflowTriggerEvents<Privilege>(eventHub: eventHub, eventStem: "privilege");
        ListenToWorkflowTriggerEvents<QueuedEmail>(eventHub: eventHub, eventStem: "queued_email");
        ListenToWorkflowTriggerEvents<Resource>(eventHub: eventHub, eventStem: "resource");
        ListenToWorkflowTriggerEvents<Role>(eventHub: eventHub, eventStem: "role");
        ListenToWorkflowTriggerEvents<ScheduledTask>(eventHub: eventHub, eventStem: "scheduled_task");
        ListenToWorkflowTriggerEvents<Script>(eventHub: eventHub, eventStem: "script");
        ListenToWorkflowTriggerEvents<SentEmail>(eventHub: eventHub, eventStem: "sent_email");
        ListenToWorkflowTriggerEvents<Submission>(eventHub: eventHub, eventStem: "submission");
        ListenToWorkflowTriggerEvents<Template>(eventHub: eventHub, eventStem: "template");
        ListenToWorkflowTriggerEvents<User>(eventHub: eventHub, eventStem: "user");
        ListenToWorkflowTriggerEvents<UserRole>(eventHub: eventHub, eventStem: "user_role");
        ListenToWorkflowTriggerEvents<WorkflowEvent>(eventHub: eventHub, eventStem: "workflow");

        eventHub.ListenToEvent(
            name: "package_import",
            handler: (IWorkflowEventCoordinationService service,
                WorkflowPackageEvent packageEvent) =>
                new ValueTask(service.RaiseEvents(
                    payload: packageEvent.Package,
                    eventName: "package_import",
                    appIdOverride: packageEvent.AppId)));
    }

    private static void ListenToWorkflowTriggerEvents<T>(
        IEventHub eventHub,
        string eventStem)
    {
        ListenToWorkflowTriggerEvent<T>(
            eventHub: eventHub,
            eventName: $"{eventStem}_add");

        ListenToWorkflowTriggerEvent<T>(
            eventHub: eventHub,
            eventName: $"{eventStem}_update");

        ListenToWorkflowTriggerEvent<T>(
            eventHub: eventHub,
            eventName: $"{eventStem}_delete");
    }

    private static void ListenToWorkflowTriggerEvent<T>(
        IEventHub eventHub,
        string eventName) =>
        eventHub.ListenToEvent(
            name: eventName,
            handler: (IWorkflowEventCoordinationService service, T payload) =>
                new ValueTask(service.RaiseEvents(
                    payload: payload,
                    eventName: eventName)));

    private static void ListenToScheduledTaskExecuteEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "scheduled_task_execute",
            handler: async (IFlowDefinitionCoordinationService service,
                ScheduledTask task) =>
            {
                _ = await service.QueueAsync(
                    flowDefinitionId: task.FlowId,
                    asUserId: task.ExecuteAs,
                    args: task.ExecutionArgs);
            });

    private static void ListenToQueuedFlowInstanceExecuteEvents(IEventHub eventHub)
    {
        ListenToQueuedFlowInstanceExecuteEvent(
            eventHub: eventHub,
            eventName: "flow_instance_data_add");

        ListenToQueuedFlowInstanceExecuteEvent(
            eventHub: eventHub,
            eventName: "flow_instance_data_update");
    }

    private static void ListenToQueuedFlowInstanceExecuteEvent(
        IEventHub eventHub,
        string eventName) =>
        eventHub.ListenToEvent(
            name: eventName,
            handler: async (IWorkflowInstanceProcessingService service,
                FlowInstanceData instance) =>
            {
                if (string.Equals(
                    a: instance?.State,
                    b: "Queued",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    await service.ExecuteWaitingQueuedInstanceByIdAsync(
                        flowInstanceDataId: instance.Id);
                }
            });

    private static WorkflowPackage ToLocalPackage(Package package) =>
        package == null
            ? null
            : new WorkflowPackage
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Category = package.Category,
                SourceApi = package.SourceApi,
                Items = package.Items?
                    .Select(selector: ToLocalPackageItem)
                    .ToArray(),
            };

    private static WorkflowPackageItem ToLocalPackageItem(
        DataPackageItem packageItem) =>
        packageItem == null
            ? null
            : new WorkflowPackageItem
            {
                Id = packageItem.Id,
                PackageId = packageItem.PackageId,
                Type = packageItem.Type,
                Data = packageItem.Data,
            };
}