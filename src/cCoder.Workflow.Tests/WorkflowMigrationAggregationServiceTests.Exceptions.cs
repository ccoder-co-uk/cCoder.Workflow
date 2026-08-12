// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapExportPackageFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<ICalendarOrchestrationService>(
                    operation: WorkflowMigrationOperation.Calendar))
            .Throws(exception: exception);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        Action action = () => service.ExportPackage(
            appId: 1,
            packageName: "Calendars");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapImportPackageWorkflowPackageAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IJsonBroker>(
                    operation: WorkflowMigrationOperation.Json))
            .Throws(exception: exception);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items =
            [
                new WorkflowPackageItem
                {
                    Type = "Workflow/FlowDefinition",
                    Data = "[]"
                }
            ]
        };

        // When
        Func<Task> action = async () => await service
            .ImportPackageWorkflowPackageAsync(appId: 1, package: package);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}