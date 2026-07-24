// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: () => { ValidateInputs(inputs: []); ExecuteListenToAllEvents(); });

    private void ExecuteListenToAllEvents()
    {
        ListenToAppEvents();
        ListenToCalendarEvents();
        ListenToFlowDefinitionEvents();
        ListenToPackageEvents();
        ListenToWorkflowTriggerEvents();
    }

    public void ListenToScheduledTaskExecuteEvents() =>
        TryCatch(operation: () => { ValidateInputs(inputs: []); ExecuteListenToScheduledTaskExecuteEvents(); });

    private void ExecuteListenToScheduledTaskExecuteEvents() =>
        ListenToScheduledTaskExecuteEventsInternal();

    public void ListenToQueuedFlowInstanceExecuteEvents() =>
        TryCatch(operation: () => { ValidateInputs(inputs: []); ExecuteListenToQueuedFlowInstanceExecuteEvents(); });

    private void ExecuteListenToQueuedFlowInstanceExecuteEvents() =>
        ListenToQueuedFlowInstanceExecuteEventsInternal();

    void ListenToAppEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    void ListenToCalendarEvents()
    {
        ListenToCalendarAddEvents();
        ListenToCalendarUpdateEvents();
        ListenToCalendarDeleteEvents();
    }

    void ListenToFlowDefinitionEvents()
    {
        ListenToFlowDefinitionDeleteEvents();
    }

    void ListenToPackageEvents() =>
        ListenToPackageImportEvents();

    void ListenToWorkflowTriggerEvents()
    {
        ListenToWorkflowTriggerEvents<App>(eventStem: "app");
        ListenToWorkflowTriggerEvents<AppCulture>(eventStem: "app_culture");
        ListenToWorkflowTriggerEvents<Calendar>(eventStem: "calendar");
        ListenToWorkflowTriggerEvents<CalendarEvent>(eventStem: "calendar_event");
        ListenToWorkflowTriggerEvents<CommonObject>(eventStem: "common_object");
        ListenToWorkflowTriggerEvents<Component>(eventStem: "component");
        ListenToWorkflowTriggerEvents<Content>(eventStem: "content");
        ListenToWorkflowTriggerEvents<Culture>(eventStem: "culture");
        ListenToWorkflowTriggerEvents<DataFile>(eventStem: "file");
        ListenToWorkflowTriggerEvents<FileContent>(eventStem: "file_content");
        ListenToWorkflowTriggerEvents<FlowDefinition>(eventStem: "flow_definition");
        ListenToWorkflowTriggerEvents<FlowInstanceData>(eventStem: "flow_instance_data");
        ListenToWorkflowTriggerEvents<Folder>(eventStem: "folder");
        ListenToWorkflowTriggerEvents<FolderRole>(eventStem: "folder_role");
        ListenToWorkflowTriggerEvents<Layout>(eventStem: "layout");
        ListenToWorkflowTriggerEvents<LogDataItem>(eventStem: "log_data_item");
        ListenToWorkflowTriggerEvents<LogEntry>(eventStem: "log_entry");
        ListenToWorkflowTriggerEvents<MailServer>(eventStem: "mail_server");
        ListenToWorkflowTriggerEvents<Package>(eventStem: "package");
        ListenToWorkflowTriggerEvents<DataPackageItem>(eventStem: "package_item");
        ListenToWorkflowTriggerEvents<Page>(eventStem: "page");
        ListenToWorkflowTriggerEvents<PageInfo>(eventStem: "page_info");
        ListenToWorkflowTriggerEvents<PageRole>(eventStem: "page_role");
        ListenToWorkflowTriggerEvents<Privilege>(eventStem: "privilege");
        ListenToWorkflowTriggerEvents<QueuedEmail>(eventStem: "queued_email");
        ListenToWorkflowTriggerEvents<Resource>(eventStem: "resource");
        ListenToWorkflowTriggerEvents<Role>(eventStem: "role");
        ListenToWorkflowTriggerEvents<ScheduledTask>(eventStem: "scheduled_task");
        ListenToWorkflowTriggerEvents<Script>(eventStem: "script");
        ListenToWorkflowTriggerEvents<SentEmail>(eventStem: "sent_email");
        ListenToWorkflowTriggerEvents<Submission>(eventStem: "submission");
        ListenToWorkflowTriggerEvents<Template>(eventStem: "template");
        ListenToWorkflowTriggerEvents<User>(eventStem: "user");
        ListenToWorkflowTriggerEvents<UserRole>(eventStem: "user_role");
        ListenToWorkflowTriggerEvents<WorkflowEvent>(eventStem: "workflow");
        ListenToWorkflowPackageImportEvents();
    }

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppCoordinationService>(
eventName: "app_add",
handler: (service, app) => service.AddAppAsync(newApp: app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppCoordinationService>(
eventName: "app_update",
handler: (service, app) => service.UpdateAppAsync(updatedApp: app));

    void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppCoordinationService>(
eventName: "app_delete",
handler: (service, app) => service.DeleteAsync(appId: app.Id));

    void ListenToCalendarAddEvents() =>
        eventHubBroker.ListenToEvent<Calendar, ICalendarEventOrchestrationService>(
eventName: "calendar_add",
handler: async (service, calendar) =>
        {
            _ = await service.AddOrUpdateCalendarEvent(
                items: calendar.Events ?? []);
        });

    void ListenToCalendarUpdateEvents() =>
        eventHubBroker.ListenToEvent<Calendar, ICalendarEventOrchestrationService>(
eventName: "calendar_update",
handler: async (service, calendar) =>
        {
            _ = await service.AddOrUpdateCalendarEvent(
                items: calendar.Events ?? []);
        });

    void ListenToCalendarDeleteEvents() =>
        eventHubBroker.ListenToEvent<Calendar, ICalendarEventOrchestrationService>(
eventName: "calendar_delete",
handler: (service, calendar) => service.DeleteAllCalendarEventAsync(
            deletedItems: calendar.Events ?? []));

    void ListenToFlowDefinitionDeleteEvents() =>
        eventHubBroker.ListenToEvent<FlowDefinition, IFlowDefinitionCoordinationService>(
eventName: "flow_definition_delete",
handler: (service, flowDefinition) => service.HandleFlowDefinitionDeleteAsync(flowDefinition: flowDefinition));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IWorkflowMigrationAggregationService>(
            eventName: "package_import",
            handler: (service, args) => service.ImportPackageWorkflowPackageAsync(
                appId: args.appId,
                package: ToLocalPackage(package: args.package)));

    void ListenToWorkflowTriggerEvents<T>(string eventStem)
    {
        ListenToWorkflowTriggerEvent<T>(eventName: $"{eventStem}_add");
        ListenToWorkflowTriggerEvent<T>(eventName: $"{eventStem}_update");
        ListenToWorkflowTriggerEvent<T>(eventName: $"{eventStem}_delete");
    }

    void ListenToWorkflowTriggerEvent<T>(string eventName) =>
        eventHubBroker.ListenToEvent<T, IWorkflowEventCoordinationService>(
eventName: eventName,
handler: (service, payload) => new ValueTask(service.RaiseEvents(payload: payload, eventName: eventName)));

    void ListenToWorkflowPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IWorkflowEventCoordinationService>(
eventName: "package_import",
handler: (service, args) => new ValueTask(service.RaiseEvents(payload: args.package, eventName: "package_import", appIdOverride: args.appId)));

    void ListenToScheduledTaskExecuteEventsInternal() =>
        eventHubBroker.ListenToEvent<ScheduledTask, IFlowDefinitionCoordinationService>(
eventName: "scheduled_task_execute",
handler: async (service, task) =>
            {
                _ = await service.QueueAsync(flowDefinitionId: task.FlowId, asUserId: task.ExecuteAs, args: task.ExecutionArgs);
            });

    void ListenToQueuedFlowInstanceExecuteEventsInternal()
    {
        ListenToQueuedFlowInstanceExecuteEvent(eventName: "flow_instance_data_add");
        ListenToQueuedFlowInstanceExecuteEvent(eventName: "flow_instance_data_update");
    }

    void ListenToQueuedFlowInstanceExecuteEvent(string eventName) =>
        eventHubBroker.ListenToEvent<FlowInstanceData, IWorkflowInstanceProcessingService>(
eventName: eventName,
handler: async (service, instance) =>
            {
                if (string.Equals(a: instance?.State, b: "Queued", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    await service.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: instance.Id);
                }
            });

    static WorkflowPackage ToLocalPackage(Package package) =>
        package == null ? null : new WorkflowPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(selector: ToLocalPackageItem)
            .ToArray(),
        };

    static WorkflowPackageItem ToLocalPackageItem(DataPackageItem packageItem) =>
        packageItem == null ? null : new WorkflowPackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data,
        };
}