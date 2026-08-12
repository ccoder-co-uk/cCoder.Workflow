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
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldImportNewCalendarsAndIgnoreExistingCalendarsAsync()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarOrchestrationService> calendarServiceMock = new();
        Calendar existing = new() { Id = 11, AppId = 7, Name = "Existing" };
        Calendar[] captured = null;

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { existing }.AsQueryable());

        calendarServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendar(
                items: It.IsAny<IEnumerable<Calendar>>()))
            .Callback<IEnumerable<Calendar>>(action: items => captured = items.ToArray())
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<Calendar>>>(
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

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items =
            [
                new()
                {
                    Type = "Core/Calendar",
                    Data = "[{\"Name\":\"Existing\"},{\"Name\":\"New\"}]"
                }
            ]
        };

        // When
        await service.ImportPackageWorkflowPackageAsync(appId: 7, package: package);

        // Then
        captured.Should().ContainSingle();
        captured.Single().Name.Should().Be(expected: "New");
        captured.Single().AppId.Should().Be(expected: 7);
        calendarServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldImportSingleCalendarObjectAsync()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarOrchestrationService> calendarServiceMock = new();

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Calendar>().AsQueryable());

        calendarServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendar(
                items: It.Is<IEnumerable<Calendar>>(
                    match: items => items.Single().Name == "Single")))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<Calendar>>>(
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

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items = [new() { Type = "Core/Calendar", Data = "{\"Name\":\"Single\"}" }]
        };

        // When
        await service.ImportPackageWorkflowPackageAsync(appId: 7, package: package);

        // Then
        calendarServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreEmptyWorkflowPackageAsync()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new(
            behavior: MockBehavior.Strict);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        await service.ImportPackageWorkflowPackageAsync(
            appId: 7,
            package: new WorkflowPackage { Items = [] });

        // Then
        brokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("calendar-1", null)]
    [InlineData(null, "Import failed")]
    public async Task ShouldRejectFailedCalendarImportAsync(
        string resultId,
        string message)
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<ICalendarOrchestrationService> calendarServiceMock = new();

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Calendar>().AsQueryable());

        calendarServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendar(
                items: It.IsAny<IEnumerable<Calendar>>()))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<Calendar>>>(
                result: [new() { Success = false, Id = resultId, Message = message }]));

        brokerMock
            .Setup(expression: broker => broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarOrchestrationService>(
                    operation: WorkflowMigrationOperation.Calendar))
            .Returns(value: calendarServiceMock.Object);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items = [new() { Type = "Core/Calendar", Data = "{\"Name\":\"Failed\"}" }]
        };

        // When
        Func<Task> action = async () => await service
            .ImportPackageWorkflowPackageAsync(appId: 7, package: package);

        // Then
        await action.Should().ThrowAsync<Exception>();
        calendarServiceMock.VerifyAll();
    }
}