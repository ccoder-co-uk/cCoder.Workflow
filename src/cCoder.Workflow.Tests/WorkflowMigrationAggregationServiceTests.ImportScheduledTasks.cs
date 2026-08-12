// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldRejectScheduledTaskWithMissingFlowAsync()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock = new();
        Mock<IFlowDefinitionOrchestrationService> flowServiceMock = new();
        Mock<IScheduledTaskOrchestrationService> taskServiceMock = new();

        flowServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<FlowDefinition>().AsQueryable());

        taskServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<ScheduledTask>().AsQueryable());

        brokerMock
            .Setup(expression: broker => broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: WorkflowMigrationOperation.FlowDefinition))
            .Returns(value: flowServiceMock.Object);

        brokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IScheduledTaskOrchestrationService>(
                    operation: WorkflowMigrationOperation.ScheduledTask))
            .Returns(value: taskServiceMock.Object);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        WorkflowPackage package = new()
        {
            Items =
            [
                new()
                {
                    Type = "Workflow/ScheduledTask",
                    Data = "{\"Name\":\"Task\",\"FlowName\":\"Missing\"}"
                }
            ]
        };

        // When
        Func<Task> action = async () => await service
            .ImportPackageWorkflowPackageAsync(appId: 7, package: package);

        // Then
        await action.Should().ThrowAsync<Exception>();
        flowServiceMock.VerifyAll();
        taskServiceMock.VerifyAll();
    }
}