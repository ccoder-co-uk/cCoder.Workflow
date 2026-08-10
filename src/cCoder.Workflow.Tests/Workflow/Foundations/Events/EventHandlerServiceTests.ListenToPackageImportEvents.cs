// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations.Events;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public sealed partial class EventHandlerServicePackageImportTests
{
    [Fact]
    public async Task ListenToPackageImportEvents_ShouldHandleSerializedHttpDataAsync()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(behavior: MockBehavior.Loose);
        Mock<IWorkflowMigrationAggregationService> migrationServiceMock = new();
        Mock<IWorkflowEventCoordinationService> eventCoordinationServiceMock = new();
        Func<IWorkflowMigrationAggregationService, WorkflowPackageEvent, ValueTask> migrationHandler = null;
        Func<IWorkflowEventCoordinationService, WorkflowPackageEvent, ValueTask> eventHandler = null;
        const int expectedAppId = 89;

        Package expectedPackage = new() { Name = "Workflows" };

        EventMessage<WorkflowPackageEvent> outboundMessage = new()
        {
            Data = new WorkflowPackageEvent
            {
                AppId = expectedAppId,
                Package = expectedPackage
            }
        };

        eventHubBrokerMock.Setup(expression: broker => broker.ListenToEvent<
                WorkflowPackageEvent,
                IWorkflowMigrationAggregationService>(
                    eventName: "package_import",
                    handler: It.IsAny<Func<IWorkflowMigrationAggregationService, WorkflowPackageEvent, ValueTask>>()))
            .Callback<string, Func<IWorkflowMigrationAggregationService, WorkflowPackageEvent, ValueTask>>(
                action: (_, handler) => migrationHandler = handler);

        eventHubBrokerMock.Setup(expression: broker => broker.ListenToEvent<
                WorkflowPackageEvent,
                IWorkflowEventCoordinationService>(
                    eventName: "package_import",
                    handler: It.IsAny<Func<IWorkflowEventCoordinationService, WorkflowPackageEvent, ValueTask>>()))
            .Callback<string, Func<IWorkflowEventCoordinationService, WorkflowPackageEvent, ValueTask>>(
                action: (_, handler) => eventHandler = handler);

        EventHandlerService service = new(eventHubBroker: eventHubBrokerMock.Object);
        string httpData = JsonSerializer.Serialize(value: outboundMessage.Data);
        WorkflowPackageEvent inboundEvent = JsonSerializer.Deserialize<WorkflowPackageEvent>(json: httpData);

        // When
        service.ListenToAllEvents();
        await migrationHandler(arg1: migrationServiceMock.Object, arg2: inboundEvent);
        await eventHandler(arg1: eventCoordinationServiceMock.Object, arg2: inboundEvent);

        // Then
        migrationServiceMock.Verify(
            expression: service => service.ImportPackageWorkflowPackageAsync(
                appId: expectedAppId,
                package: It.Is<WorkflowPackage>(match: package => package.Name == "Workflows")),
            times: Times.Once);

        eventCoordinationServiceMock.Verify(
            expression: service => service.RaiseEvents(
                payload: It.Is<Package>(match: package => package.Name == "Workflows"),
                eventName: "package_import",
                appIdOverride: expectedAppId),
            times: Times.Once);
    }
}