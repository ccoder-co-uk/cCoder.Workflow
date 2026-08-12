// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

#pragma warning disable STXFORMAT009
public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public void ShouldExportCalendarsForApp()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarOrchestrationService> calendarServiceMock = new();
        Calendar included = new() { AppId = 7, Name = "Included" };
        Calendar excluded = new() { AppId = 8, Name = "Excluded" };

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { included, excluded }.AsQueryable());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarOrchestrationService>(
                    operation: WorkflowMigrationOperation.Calendar))
            .Returns(value: calendarServiceMock.Object);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        WorkflowPackage result = service.ExportPackage(
            appId: 7,
            packageName: "Calendars");

        // Then
        result.Name.Should().Be(expected: "Calendars");
        result.Items.Should().ContainSingle();
        result.Items.Single().Data.Should().Contain(expected: "Included");
        result.Items.Single().Data.Should().NotContain(unexpected: "Excluded");
        calendarServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldExportCalendarEventsForApp()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarEventOrchestrationService> eventServiceMock = new();
        Calendar includedCalendar = new() { AppId = 7, Name = "Included calendar" };
        Calendar excludedCalendar = new() { AppId = 8, Name = "Excluded calendar" };

        CalendarEvent included = new()
        {
            Name = "Included event",
            Calendar = includedCalendar
        };

        CalendarEvent excluded = new()
        {
            Name = "Excluded event",
            Calendar = excludedCalendar
        };

        eventServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { included, excluded, new CalendarEvent() }
                .AsQueryable());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarEventOrchestrationService>(
                    operation: WorkflowMigrationOperation.CalendarEvent))
            .Returns(value: eventServiceMock.Object);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        WorkflowPackage result = service.ExportPackage(
            appId: 7,
            packageName: "CalendarEvents");

        // Then
        result.Items.Single().Data.Should().Contain(expected: "Included event");
        result.Items.Single().Data.Should().NotContain(unexpected: "Excluded event");
        eventServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldExportFlowDefinitionsForApp()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<IFlowDefinitionOrchestrationService> flowServiceMock = new();
        App app = new() { Name = "Process" };

        FlowDefinition included = new()
        {
            AppId = 7,
            App = app,
            Name = "Included flow"
        };

        FlowDefinition excluded = new()
        {
            AppId = 8,
            App = app,
            Name = "Excluded flow"
        };

        flowServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { included, excluded }.AsQueryable());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: WorkflowMigrationOperation.FlowDefinition))
            .Returns(value: flowServiceMock.Object);

        brokerMock
            .Setup(expression: broker => broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        WorkflowPackage result = service.ExportPackage(
            appId: 7,
            packageName: "Workflows");

        // Then
        result.Items.Single().Data.Should().Contain(expected: "Included flow");
        result.Items.Single().Data.Should().NotContain(unexpected: "Excluded flow");
        flowServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldExportScheduledTasksForApp()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<IScheduledTaskOrchestrationService> taskServiceMock = new();
        FlowDefinition flow = new() { Name = "Flow" };
        ScheduledTask included = new() { AppId = 7, Name = "Included task", Flow = flow };
        ScheduledTask excluded = new() { AppId = 8, Name = "Excluded task", Flow = flow };

        taskServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { included, excluded }.AsQueryable());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IScheduledTaskOrchestrationService>(
                    operation: WorkflowMigrationOperation.ScheduledTask))
            .Returns(value: taskServiceMock.Object);

        brokerMock
            .Setup(expression: broker => broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        WorkflowPackage result = service.ExportPackage(
            appId: 7,
            packageName: "ScheduledTasks");

        // Then
        result.Items.Single().Data.Should().Contain(expected: "Included task");
        result.Items.Single().Data.Should().NotContain(unexpected: "Excluded task");
        taskServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT009