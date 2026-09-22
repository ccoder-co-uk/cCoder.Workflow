// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using FluentAssertions;
using Moq;
using System.Text.Json;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void WorkflowEventStartup_WhenInspected_IsExposedFromIEventHubExtensions()
    {
        // Given
        Type workflowAssemblyMarker = typeof(cCoder.Workflow.IServiceCollectionExtensions);

        // When
        Type eventHubExtensionsType = workflowAssemblyMarker.Assembly
            .GetType(name: "cCoder.Workflow.IEventHubExtensions");

        // Then
        eventHubExtensionsType
            .Should()
            .NotBeNull();

        eventHubExtensionsType!
            .GetMethods()
            .Where(predicate: method =>
                method.IsPublic &&
                method.IsStatic &&
                method.Name == "ListenToWorkflowEvents")
            .Select(selector: method => method.GetParameters()[0].ParameterType)
            .Should()
            .ContainSingle(predicate: receiverType => receiverType == typeof(IEventHub));
    }

    [Fact]
    public void WorkflowEventListening_WhenInspected_HasNoIntermediateListenerInfrastructure()
    {
        // Given
        Type workflowAssemblyMarker = typeof(cCoder.Workflow.IServiceCollectionExtensions);

        // When
        Type[] listenerInfrastructure = workflowAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Name is "WorkflowEventHandlers" or
                    "IWorkflowEventHandlers" or
                    "EventHandlerService" or
                    "IEventHandlerService" or
                    "EventHubBroker" or
                    "IEventHubBroker")
            .ToArray();

        // Then
        listenerInfrastructure
            .Should()
            .BeEmpty(
                because: "IEventHubExtensions should wire Workflow listeners directly on the established event hub boundary");
    }

    [Fact]
    public void WorkflowEvents_WhenStarted_RegisterTheExistingWorkflowListenerMapOnce()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        List<(string EventName, Type PayloadType, Type ServiceType)> expectedRegistrations =
        [
            ("app_add", typeof(App), typeof(IAppCoordinationService)),
            ("app_update", typeof(App), typeof(IAppCoordinationService)),
            ("app_delete", typeof(App), typeof(IAppCoordinationService)),
            ("calendar_add", typeof(Calendar), typeof(ICalendarEventOrchestrationService)),
            ("calendar_update", typeof(Calendar), typeof(ICalendarEventOrchestrationService)),
            ("calendar_delete", typeof(Calendar), typeof(ICalendarEventOrchestrationService)),
            ("flow_definition_delete", typeof(FlowDefinition), typeof(IFlowDefinitionCoordinationService)),
            ("package_import", typeof(WorkflowPackageEvent), typeof(IWorkflowMigrationAggregationService)),
        ];

        (string EventStem, Type PayloadType)[] triggerTypes =
        [
            ("app", typeof(App)),
            ("app_culture", typeof(AppCulture)),
            ("calendar", typeof(Calendar)),
            ("calendar_event", typeof(CalendarEvent)),
            ("common_object", typeof(CommonObject)),
            ("component", typeof(Component)),
            ("content", typeof(Content)),
            ("culture", typeof(Culture)),
            ("file", typeof(DataFile)),
            ("file_content", typeof(FileContent)),
            ("flow_definition", typeof(FlowDefinition)),
            ("flow_instance_data", typeof(FlowInstanceData)),
            ("folder", typeof(Folder)),
            ("folder_role", typeof(FolderRole)),
            ("layout", typeof(Layout)),
            ("log_data_item", typeof(LogDataItem)),
            ("log_entry", typeof(LogEntry)),
            ("mail_server", typeof(MailServer)),
            ("package", typeof(Package)),
            ("package_item", typeof(DataPackageItem)),
            ("page", typeof(Page)),
            ("page_info", typeof(PageInfo)),
            ("page_role", typeof(PageRole)),
            ("privilege", typeof(Privilege)),
            ("queued_email", typeof(QueuedEmail)),
            ("resource", typeof(Resource)),
            ("role", typeof(Role)),
            ("scheduled_task", typeof(ScheduledTask)),
            ("script", typeof(Script)),
            ("sent_email", typeof(SentEmail)),
            ("submission", typeof(Submission)),
            ("template", typeof(Template)),
            ("user", typeof(User)),
            ("user_role", typeof(UserRole)),
            ("workflow", typeof(WorkflowEvent)),
        ];

        foreach ((string eventStem, Type payloadType) in triggerTypes)
        {
            expectedRegistrations.Add(
                item: ($"{eventStem}_add", payloadType, typeof(IWorkflowEventCoordinationService)));

            expectedRegistrations.Add(
                item: ($"{eventStem}_update", payloadType, typeof(IWorkflowEventCoordinationService)));

            expectedRegistrations.Add(
                item: ($"{eventStem}_delete", payloadType, typeof(IWorkflowEventCoordinationService)));
        }

        expectedRegistrations.Add(
            item: ("package_import", typeof(WorkflowPackageEvent), typeof(IWorkflowEventCoordinationService)));

        expectedRegistrations.Add(
            item: ("scheduled_task_execute", typeof(ScheduledTask), typeof(IFlowDefinitionCoordinationService)));

        expectedRegistrations.Add(
            item: ("flow_instance_data_add", typeof(FlowInstanceData), typeof(IWorkflowInstanceProcessingService)));

        expectedRegistrations.Add(
            item: ("flow_instance_data_update", typeof(FlowInstanceData), typeof(IWorkflowInstanceProcessingService)));

        // When
        eventHubMock.Object.ListenToWorkflowEvents();
        eventHubMock.Object.ListenToWorkflowEvents();

        // Then
        eventHubMock.Invocations
            .Select(selector: invocation =>
                (
                    EventName: (string)invocation.Arguments[0],
                    PayloadType: invocation.Method.GetGenericArguments()[0],
                    ServiceType: invocation.Method.GetGenericArguments()[1]
                ))
            .Should()
            .BeEquivalentTo(expectation: expectedRegistrations);
    }

    [Fact]
    public async Task PackageImportEvents_WhenRaised_PreserveMigrationAndWorkflowCallbacksAsync()
    {
        // Given
        const int expectedAppId = 89;
        Mock<IEventHub> eventHubMock = new();
        Mock<IWorkflowMigrationAggregationService> migrationServiceMock = new();
        Mock<IWorkflowEventCoordinationService> eventCoordinationServiceMock = new();
        Package expectedPackage = new() { Name = "Workflows" };

        EventMessage<WorkflowPackageEvent> outboundMessage = new()
        {
            Data = new WorkflowPackageEvent
            {
                AppId = expectedAppId,
                Package = expectedPackage,
            },
        };

        string httpData = JsonSerializer.Serialize(value: outboundMessage.Data);

        WorkflowPackageEvent inboundEvent =
            JsonSerializer.Deserialize<WorkflowPackageEvent>(json: httpData);

        eventHubMock.Object.ListenToWorkflowEvents();

        Func<IWorkflowMigrationAggregationService, WorkflowPackageEvent, ValueTask> migrationHandler =
            (Func<IWorkflowMigrationAggregationService, WorkflowPackageEvent, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "package_import",
                serviceType: typeof(IWorkflowMigrationAggregationService));

        Func<IWorkflowEventCoordinationService, WorkflowPackageEvent, ValueTask> eventHandler =
            (Func<IWorkflowEventCoordinationService, WorkflowPackageEvent, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "package_import",
                serviceType: typeof(IWorkflowEventCoordinationService));

        // When
        await migrationHandler(
            arg1: migrationServiceMock.Object,
            arg2: inboundEvent);

        await eventHandler(
            arg1: eventCoordinationServiceMock.Object,
            arg2: inboundEvent);

        // Then
        migrationServiceMock.Verify(
            expression: service => service.ImportPackageWorkflowPackageAsync(
                appId: expectedAppId,
                workflowPackage: It.Is<WorkflowPackage>(match: package =>
                    package.Name == "Workflows")),
            times: Times.Once);

        eventCoordinationServiceMock.Verify(
            expression: service => service.RaiseEvents(
                payload: It.Is<Package>(match: package =>
                    package.Name == "Workflows"),
                eventName: "package_import",
                appIdOverride: expectedAppId),
            times: Times.Once);
    }

    [Fact]
    public async Task QueuedFlowInstanceEvents_WhenRaised_ExecuteOnlyQueuedInstancesAsync()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        Mock<IWorkflowInstanceProcessingService> processingServiceMock =
            new(behavior: MockBehavior.Strict);

        FlowInstanceData queuedAddInstance =
            new() { Id = Guid.NewGuid(), State = "Queued" };

        FlowInstanceData queuedUpdateInstance =
            new() { Id = Guid.NewGuid(), State = "Queued" };

        FlowInstanceData executingInstance =
            new() { Id = Guid.NewGuid(), State = "Executing" };

        processingServiceMock
            .Setup(expression: service =>
                service.ExecuteWaitingQueuedInstanceByIdAsync(
                    flowInstanceDataId: queuedAddInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        processingServiceMock
            .Setup(expression: service =>
                service.ExecuteWaitingQueuedInstanceByIdAsync(
                    flowInstanceDataId: queuedUpdateInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToWorkflowEvents();

        Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask> addHandler =
            (Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "flow_instance_data_add",
                serviceType: typeof(IWorkflowInstanceProcessingService));

        Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask> updateHandler =
            (Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "flow_instance_data_update",
                serviceType: typeof(IWorkflowInstanceProcessingService));

        // When
        await addHandler(
            arg1: processingServiceMock.Object,
            arg2: queuedAddInstance);

        await updateHandler(
            arg1: processingServiceMock.Object,
            arg2: queuedUpdateInstance);

        await updateHandler(
            arg1: processingServiceMock.Object,
            arg2: executingInstance);

        // Then
        processingServiceMock.Verify(
            expression: service => service.ExecuteWaitingQueuedInstanceByIdAsync(
                flowInstanceDataId: queuedAddInstance.Id),
            times: Times.Once);

        processingServiceMock.Verify(
            expression: service => service.ExecuteWaitingQueuedInstanceByIdAsync(
                flowInstanceDataId: queuedUpdateInstance.Id),
            times: Times.Once);

        processingServiceMock.VerifyNoOtherCalls();
    }

    private static Delegate GetHandler(
        Mock<IEventHub> eventHubMock,
        string eventName,
        Type serviceType) =>
        (Delegate)eventHubMock.Invocations
            .Single(predicate: invocation =>
                invocation.Arguments[0] as string == eventName &&
                invocation.Method.GetGenericArguments()[1] == serviceType)
            .Arguments[1];
}