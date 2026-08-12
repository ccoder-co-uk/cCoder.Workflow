// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Models.Results;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public async Task ImportPackage_ShouldImportCurrentWorkflowItemsInDependencyOrder()
    {
        // Given
        const int appId = 7;
        Guid persistedFlowId = Guid.NewGuid();
        List<FlowDefinition> persistedFlows = [];
        bool flowImportCompleted = false;

        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<IFlowDefinitionOrchestrationService> flowServiceMock =
            new(behavior: MockBehavior.Strict);

        Mock<IScheduledTaskOrchestrationService> taskServiceMock =
            new(behavior: MockBehavior.Strict);

        Mock<IAuthorizationBroker> authorizationBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILogger<WorkflowMigrationAggregationService>> loggerMock = new();

        IEnumerable<Result<FlowDefinition>> flowResults =
            [new Result<FlowDefinition> { Success = true }];

        IEnumerable<Result<ScheduledTask>> taskResults =
            [new Result<ScheduledTask> { Success = true }];

        IQueryable<FlowDefinition> persistedFlowQuery =
            persistedFlows.AsQueryable();

        flowServiceMock.Setup(expression: service =>
            service.GetAll(ignoreFilters: true))
            .Returns(value: persistedFlowQuery);

        flowServiceMock.Setup(expression: service =>
            service.AddOrUpdateFlowDefinition(items:
                It.IsAny<IEnumerable<FlowDefinition>>()))
            .Callback<IEnumerable<FlowDefinition>>(action: flows =>
            {
                FlowDefinition flow = flows.Single();
                flow.Id = persistedFlowId;
                persistedFlows.Add(item: flow);
                flowImportCompleted = true;
            })
            .Returns(value: ValueTask.FromResult(result: flowResults));

        taskServiceMock.Setup(expression: service =>
            service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<ScheduledTask>()
                .AsQueryable());

        taskServiceMock.Setup(expression: service =>
            service.AddOrUpdateScheduledTask(items:
                It.IsAny<IEnumerable<ScheduledTask>>()))
            .Callback<IEnumerable<ScheduledTask>>(action: tasks =>
            {
                Assert.True(
                    condition: flowImportCompleted,
                    userMessage: "The flow must be persisted before its scheduled task.");

                ScheduledTask task = tasks.Single();
                Assert.Equal(expected: persistedFlowId, actual: task.FlowId);
            })
            .Returns(value: ValueTask.FromResult(result: taskResults));

        authorizationBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: new User { Id = "AcceptanceAdmin" });

        brokerMock.Setup(expression: broker =>
            broker.GetOperationService<IJsonBroker>(
                operation: WorkflowMigrationOperation.Json))
            .Returns(value: new JsonBroker());

        brokerMock.Setup(expression: broker =>
            broker.GetOperationService<IFlowDefinitionOrchestrationService>(
                operation: WorkflowMigrationOperation.FlowDefinition))
            .Returns(value: flowServiceMock.Object);

        brokerMock.Setup(expression: broker =>
            broker.GetOperationService<IScheduledTaskOrchestrationService>(
                operation: WorkflowMigrationOperation.ScheduledTask))
            .Returns(value: taskServiceMock.Object);

        brokerMock.Setup(expression: broker =>
            broker.GetOperationService<IAuthorizationBroker>(
                operation: WorkflowMigrationOperation.Authorization))
            .Returns(value: authorizationBrokerMock.Object);

        brokerMock.Setup(expression: broker =>
            broker.GetOperationService<ILogger<WorkflowMigrationAggregationService>>(
                operation: WorkflowMigrationOperation.Logging))
            .Returns(value: loggerMock.Object);

        WorkflowPackage package = new()
        {
            Items =
            [
                new WorkflowPackageItem
                {
                    Type = "Workflow/FlowDefinition",
                    Data = "[{\"Name\":\"Baseline\",\"DefinitionJson\":\"{}\"}]"
                },
                new WorkflowPackageItem
                {
                    Type = "Workflow/ScheduledTask",
                    Data = "[{\"Name\":\"Daily\",\"FlowName\":\"Baseline\","
                        + "\"ExecutionArgs\":\"{}\",\"ScheduleInTicks\":864000000000}]"
                }
            ]
        };

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        await service.ImportPackageWorkflowPackageAsync(
            appId: appId,
            package: package);

        // Then
        flowServiceMock.VerifyAll();
        taskServiceMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }
}