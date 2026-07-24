// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Brokers;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Dependencies.HostedServices;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Workflow.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using AuthorizationDependency = cCoder.Workflow.Dependencies.AuthorizationDependency;
using IAuthorizationDependency = cCoder.Workflow.Dependencies.IAuthorizationDependency;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;
using JsonBroker = cCoder.Workflow.Brokers.JsonBroker;
using IJsonDependency = cCoder.Workflow.Dependencies.IJsonDependency;
using JsonDependency = cCoder.Workflow.Dependencies.JsonDependency;
using DataFile = cCoder.Data.Models.DMS.File;


namespace cCoder.Workflow;

public static partial class IServiceCollectionExtensions
{
    public static void AddWorkflowWeb(
        this IServiceCollection services,
        Action<WorkflowConfiguration> newConfigure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredWorkflowWeb(newConfigure: (_, configuration) => newConfigure?.Invoke(obj: configuration), builder: builder);

    public static void AddWorkflowHostedServices(
        this IServiceCollection services,
        Action<WorkflowConfiguration> newConfigure = null) =>
        services.AddConfiguredWorkflowHostedServices(newConfigure: (_, configuration) => newConfigure?.Invoke(obj: configuration));

    internal static void AddWorkflow(IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddEventHandlers();
    }

    internal static void AddWorkflowWeb(IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        AddWorkflow(services: services);
        services.AddTransient<IFlowDefinitionAggregationService, FlowDefinitionAggregationService>();
    }

    internal static void AddWorkflowHostedServices(IServiceCollection services)
    {
        AddWorkflow(services: services);
        services.AddSingleton<IInstanceMaintenanceBackgroundServiceDependency, InstanceMaintenanceBackgroundServiceDependency>();

        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<IInstanceMaintenanceBackgroundServiceDependency>());

        services.AddSingleton<IQueueInstanceBackgroundServiceDependency, QueueInstanceBackgroundServiceDependency>();

        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<IQueueInstanceBackgroundServiceDependency>());

        services.AddSingleton<IScheduledTaskRunnerBackgroundServiceDependency, ScheduledTaskRunnerBackgroundServiceDependency>();

        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<IScheduledTaskRunnerBackgroundServiceDependency>());
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<AppCulture>();
        services.AddEventingForType<Calendar>();
        services.AddEventingForType<CalendarEvent>();
        services.AddEventingForType<CommonObject>();
        services.AddEventingForType<Component>();
        services.AddEventingForType<Content>();
        services.AddEventingForType<Culture>();
        services.AddEventingForType<DataFile>();
        services.AddEventingForType<FileContent>();
        services.AddEventingForType<FlowDefinition>();
        services.AddEventingForType<FlowInstanceData>();
        services.AddEventingForType<Folder>();
        services.AddEventingForType<FolderRole>();
        services.AddEventingForType<Layout>();
        services.AddEventingForType<LogDataItem>();
        services.AddEventingForType<LogEntry>();
        services.AddEventingForType<MailServer>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<(int, Package)>();
        services.AddEventingForType<Page>();
        services.AddEventingForType<PageInfo>();
        services.AddEventingForType<PageRole>();
        services.AddEventingForType<Privilege>();
        services.AddEventingForType<QueuedEmail>();
        services.AddEventingForType<Resource>();
        services.AddEventingForType<Role>();
        services.AddEventingForType<ScheduledTask>();
        services.AddEventingForType<Script>();
        services.AddEventingForType<SentEmail>();
        services.AddEventingForType<Submission>();
        services.AddEventingForType<Template>();
        services.AddEventingForType<User>();
        services.AddEventingForType<UserRole>();
        services.AddEventingForType<WorkflowEvent>();
        services.AddEventingForType<cCoder.Data.Models.Workflow.FlowDefinition>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IFlowDefinitionServiceProviderBroker, FlowDefinitionServiceProviderBroker>();
        services.AddTransient<IWorkflowMigrationServiceProviderBroker, WorkflowMigrationServiceProviderBroker>();
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IFlowDefinitionEventBroker, FlowDefinitionEventBroker>();
        services.AddTransient<IFlowInstanceDataEventBroker, FlowInstanceDataEventBroker>();
        services.AddTransient<ICalendarEntityEventBroker, CalendarEntityEventBroker>();
        services.AddTransient<ICalendarEventEventBroker, CalendarEventEventBroker>();
        services.AddTransient<IScheduledTaskEventBroker, ScheduledTaskEventBroker>();
        services.AddTransient<IWorkflowEventEventBroker, WorkflowEventEventBroker>();
        services.AddTransient<ICalendarBroker, CalendarBroker>();
        services.AddTransient<ICalendarEventBroker, CalendarEventBroker>();
        services.AddTransient<IFlowDefinitionBroker, FlowDefinitionBroker>();
        services.AddTransient<IFlowInstanceDataBroker, FlowInstanceDataBroker>();
        services.AddTransient<IScheduledTaskBroker, ScheduledTaskBroker>();
        services.AddTransient<IWorkflowInstanceManagementBroker, WorkflowInstanceManagementBroker>();
        services.AddTransient<IWorkflowEventBroker, WorkflowEventBroker>();
        services.AddTransient<IAuthorizationDependency, AuthorizationDependency>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonDependency, JsonDependency>();
        services.AddTransient<IJsonBroker, JsonBroker>();
    }

    private static void AddCoordinations(this IServiceCollection services)
    {
        services.AddTransient<IFlowDefinitionCoordinationService, FlowDefinitionCoordinationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IWorkflowAppExposure, WorkflowAppExposure>();
        services.AddTransient<IWorkflowPackageManager, WorkflowPackageManager>();
        services.AddTransient<IWorkflowEventHandlers, WorkflowEventHandlers>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<ICalendarService, CalendarService>();
        services.AddTransient<ICalendarEventService, CalendarEventService>();
        services.AddTransient<IFlowDefinitionService, FlowDefinitionService>();
        services.AddTransient<IFlowInstanceDataService, FlowInstanceDataService>();
        services.AddTransient<IScheduledTaskService, ScheduledTaskService>();
        services.AddTransient<IWorkflowMetadataTypeService, WorkflowMetadataTypeService>();
        services.AddTransient<ICalendarEntityEventService, CalendarEntityEventService>();
        services.AddTransient<ICalendarEventEventService, CalendarEventEventService>();
        services.AddTransient<IWorkflowEventService, WorkflowEventService>();
        services.AddTransient<IFlowDefinitionEventService, FlowDefinitionEventService>();
        services.AddTransient<IFlowInstanceDataEventService, FlowInstanceDataEventService>();
        services.AddTransient<IScheduledTaskEventService, ScheduledTaskEventService>();
        services.AddTransient<IWorkflowEventEventService, WorkflowEventEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppCoordinationService, AppCoordinationService>();
        services.AddTransient<ICalendarOrchestrationService, CalendarOrchestrationService>();
        services.AddTransient<ICalendarEventOrchestrationService, CalendarEventOrchestrationService>();
        services.AddTransient<IWorkflowEventCoordinationService, WorkflowEventCoordinationService>();
        services.AddTransient<IWorkflowMigrationAggregationService, WorkflowMigrationAggregationService>();
        services.AddTransient<IFlowDefinitionOrchestrationService, FlowDefinitionOrchestrationService>();
        services.AddTransient<IFlowQueueOrchestrationService, FlowQueueOrchestrationService>();
        services.AddTransient<IFlowInstanceDataOrchestrationService, FlowInstanceDataOrchestrationService>();
        services.AddTransient<IScheduledTaskOrchestrationService, ScheduledTaskOrchestrationService>();
        services.AddTransient<ITaskRunnerOrchestrationService, TaskRunnerOrchestrationService>();
        services.AddTransient<IWorkflowInstanceProcessingService, WorkflowInstanceProcessingService>();
        services.AddTransient<IWorkflowEventOrchestrationService, WorkflowEventOrchestrationService>();

        services.AddKeyedTransient<IFlowDefinitionOrchestrationService>(
            serviceKey: FlowDefinitionOperation.Crud,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IFlowDefinitionOrchestrationService>());

        services.AddKeyedTransient<IFlowDefinitionCoordinationService>(
            serviceKey: FlowDefinitionOperation.Queue,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IFlowDefinitionCoordinationService>());

        services.AddKeyedTransient<IWorkflowMetadataTypeService>(
            serviceKey: FlowDefinitionOperation.Metadata,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IWorkflowMetadataTypeService>());

        services.AddKeyedTransient<IAuthorizationBroker>(
            serviceKey: FlowDefinitionOperation.Authorization,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IAuthorizationBroker>());

        services.AddKeyedTransient<Config>(
            serviceKey: FlowDefinitionOperation.Configuration,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<Config>());

        services.AddKeyedTransient<ICalendarOrchestrationService>(
            serviceKey: WorkflowMigrationOperation.Calendar,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ICalendarOrchestrationService>());

        services.AddKeyedTransient<ICalendarEventOrchestrationService>(
            serviceKey: WorkflowMigrationOperation.CalendarEvent,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ICalendarEventOrchestrationService>());

        services.AddKeyedTransient<IFlowDefinitionOrchestrationService>(
            serviceKey: WorkflowMigrationOperation.FlowDefinition,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IFlowDefinitionOrchestrationService>());

        services.AddKeyedTransient<IJsonBroker>(
            serviceKey: WorkflowMigrationOperation.Json,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<IJsonBroker>());

        services.AddKeyedTransient<ILogger<WorkflowMigrationAggregationService>>(
            serviceKey: WorkflowMigrationOperation.Logging,
            implementationFactory: static (serviceProvider, _) =>
                serviceProvider.GetRequiredService<ILogger<WorkflowMigrationAggregationService>>());
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IBaselineProcessingService, BaselineProcessingService>();
        services.AddTransient<ICalendarEntityEventProcessingService, CalendarEntityEventProcessingService>();
        services.AddTransient<ICalendarEventEventProcessingService, CalendarEventEventProcessingService>();
        services.AddTransient<ICalendarEventProcessingService, CalendarEventProcessingService>();
        services.AddTransient<ICalendarProcessingService, CalendarProcessingService>();
        services.AddTransient<IFlowDefinitionEventProcessingService, FlowDefinitionEventProcessingService>();
        services.AddTransient<IFlowDefinitionProcessingService, FlowDefinitionProcessingService>();
        services.AddTransient<IFlowInstanceDataEventProcessingService, FlowInstanceDataEventProcessingService>();
        services.AddTransient<IFlowInstanceDataProcessingService, FlowInstanceDataProcessingService>();
        services.AddTransient<IScheduledTaskEventProcessingService, ScheduledTaskEventProcessingService>();
        services.AddTransient<IScheduledTaskProcessingService, ScheduledTaskProcessingService>();
        services.AddTransient<IWorkflowEventEventProcessingService, WorkflowEventEventProcessingService>();
        services.AddTransient<IWorkflowEventProcessingService, WorkflowEventProcessingService>();
    }
}