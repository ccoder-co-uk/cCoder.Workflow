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
using DataFile = cCoder.Data.Models.DMS.File;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.Workflow.Services.Foundations.Events;

internal class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ListenToAppEvents();
        ListenToFlowDefinitionEvents();
        ListenToPackageEvents();
        ListenToWorkflowTriggerEvents();
    }

    public void ListenToScheduledTaskExecuteEvents() =>
        ListenToScheduledTaskExecuteEventsInternal();

    void ListenToAppEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    void ListenToFlowDefinitionEvents()
    {
        ListenToFlowDefinitionDeleteEvents();
    }

    void ListenToPackageEvents() => ListenToPackageImportEvents();

    void ListenToWorkflowTriggerEvents()
    {
        ListenToWorkflowTriggerEvents<App>("app");
        ListenToWorkflowTriggerEvents<AppCulture>("app_culture");
        ListenToWorkflowTriggerEvents<Calendar>("calendar");
        ListenToWorkflowTriggerEvents<CalendarEvent>("calendar_event");
        ListenToWorkflowTriggerEvents<CommonObject>("common_object");
        ListenToWorkflowTriggerEvents<Component>("component");
        ListenToWorkflowTriggerEvents<Content>("content");
        ListenToWorkflowTriggerEvents<Culture>("culture");
        ListenToWorkflowTriggerEvents<DataFile>("file");
        ListenToWorkflowTriggerEvents<FileContent>("file_content");
        ListenToWorkflowTriggerEvents<FlowDefinition>("flow_definition");
        ListenToWorkflowTriggerEvents<FlowInstanceData>("flow_instance_data");
        ListenToWorkflowTriggerEvents<Folder>("folder");
        ListenToWorkflowTriggerEvents<FolderRole>("folder_role");
        ListenToWorkflowTriggerEvents<Layout>("layout");
        ListenToWorkflowTriggerEvents<LogDataItem>("log_data_item");
        ListenToWorkflowTriggerEvents<LogEntry>("log_entry");
        ListenToWorkflowTriggerEvents<MailServer>("mail_server");
        ListenToWorkflowTriggerEvents<Package>("package");
        ListenToWorkflowTriggerEvents<DataPackageItem>("package_item");
        ListenToWorkflowTriggerEvents<Page>("page");
        ListenToWorkflowTriggerEvents<PageInfo>("page_info");
        ListenToWorkflowTriggerEvents<PageRole>("page_role");
        ListenToWorkflowTriggerEvents<Privilege>("privilege");
        ListenToWorkflowTriggerEvents<QueuedEmail>("queued_email");
        ListenToWorkflowTriggerEvents<Resource>("resource");
        ListenToWorkflowTriggerEvents<Role>("role");
        ListenToWorkflowTriggerEvents<ScheduledTask>("scheduled_task");
        ListenToWorkflowTriggerEvents<Script>("script");
        ListenToWorkflowTriggerEvents<SentEmail>("sent_email");
        ListenToWorkflowTriggerEvents<Submission>("submission");
        ListenToWorkflowTriggerEvents<Template>("template");
        ListenToWorkflowTriggerEvents<User>("user");
        ListenToWorkflowTriggerEvents<UserRole>("user_role");
        ListenToWorkflowTriggerEvents<WorkflowEvent>("workflow");
        ListenToWorkflowPackageImportEvents();
        ListenToScheduledTaskExecuteEventsInternal();
    }

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_add",
            (service, app) => service.AddAsync(app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_update",
            (service, app) => service.UpdateAsync(app));

    void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_delete",
            (service, app) => service.DeleteAsync(app.Id));

    void ListenToFlowDefinitionDeleteEvents() =>
        eventHubBroker.ListenToEvent<FlowDefinition, IFlowDefinitionCoordinationService>(
            "flow_definition_delete",
            (service, flowDefinition) => service.HandleFlowDefinitionDeleteAsync(flowDefinition));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IWorkflowMigrationAggregationService>(
            "package_import",
            (service, args) => service.ImportPackageAsync(args.appId, ToLocalPackage(args.package)));

    void ListenToWorkflowTriggerEvents<T>(string eventStem)
    {
        ListenToWorkflowTriggerEvent<T>($"{eventStem}_add");
        ListenToWorkflowTriggerEvent<T>($"{eventStem}_update");
        ListenToWorkflowTriggerEvent<T>($"{eventStem}_delete");
    }

    void ListenToWorkflowTriggerEvent<T>(string eventName) =>
        eventHubBroker.ListenToEvent<T, IEventHandlingOrchestrationService>(
            eventName,
            (service, payload) => new ValueTask(service.RaiseEvents(payload, eventName)));

    void ListenToWorkflowPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IEventHandlingOrchestrationService>(
            "package_import",
            (service, args) => new ValueTask(service.RaiseEvents(args.package, "package_import", args.appId)));

    void ListenToScheduledTaskExecuteEventsInternal() =>
        eventHubBroker.ListenToEvent<ScheduledTask, IFlowDefinitionCoordinationService>(
            "scheduled_task_execute",
            async (service, task) =>
            {
                _ = await service.QueueAsync(task.FlowId, task.ExecuteAs, task.ExecutionArgs);
            });

    static WorkflowPackage ToLocalPackage(Package package) =>
        package == null ? null : new WorkflowPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(ToLocalPackageItem).ToArray(),
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
