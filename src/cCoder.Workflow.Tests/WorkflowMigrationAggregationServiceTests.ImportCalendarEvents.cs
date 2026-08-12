// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Models.Results;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Orchestrations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldImportOnlyNewCalendarEventsWithKnownCalendarsAsync()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarOrchestrationService> calendarServiceMock = new();
        Mock<ICalendarEventOrchestrationService> eventServiceMock = new();
        Mock<ILogger<WorkflowMigrationAggregationService>> loggerMock = new();
        Calendar calendar = new() { Id = 11, AppId = 7, Name = "Calendar" };

        CalendarEvent existing = new()
        {
            Id = 12,
            Name = "Existing",
            Calendar = calendar,
            CalendarId = calendar.Id
        };

        CalendarEvent[] captured = null;

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { calendar }.AsQueryable());

        eventServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { existing }.AsQueryable());

        eventServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendarEvent(
                items: It.IsAny<IEnumerable<CalendarEvent>>()))
            .Callback<IEnumerable<CalendarEvent>>(action: items => captured = items.ToArray())
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<CalendarEvent>>>(
                result: [new() { Success = true }]));

        brokerMock
            .Setup(expression: broker => broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarOrchestrationService>(
                    operation: WorkflowMigrationOperation.Calendar))
            .Returns(value: calendarServiceMock.Object);

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarEventOrchestrationService>(
                    operation: WorkflowMigrationOperation.CalendarEvent))
            .Returns(value: eventServiceMock.Object);

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ILogger<WorkflowMigrationAggregationService>>(
                    operation: WorkflowMigrationOperation.Logging))
            .Returns(value: loggerMock.Object);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items =
            [
                new()
                {
                    Type = "Core/CalendarEvent",
                    Data = "["
                        + "{\"Name\":\"New\",\"CalendarName\":\"Calendar\"},"
                        + "{\"Name\":\"Existing\",\"CalendarName\":\"Calendar\"},"
                        + "{\"Name\":\"Unknown\",\"CalendarName\":\"Missing\"}"
                        + "]"
                }
            ]
        };

        // When
        await service.ImportPackageWorkflowPackageAsync(appId: 7, package: package);

        // Then
        captured.Should().ContainSingle();
        captured.Single().Name.Should().Be(expected: "New");
        captured.Single().CalendarId.Should().Be(expected: calendar.Id);
        calendarServiceMock.VerifyAll();
        eventServiceMock.VerifyAll();
    }
}