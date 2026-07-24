// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Objects.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public sealed class WorkflowInstanceManagementOrchestrationServiceTests
{
    private readonly Mock<IWorkflowInstanceManagementBroker> workflowInstanceManagementBrokerMock;
    private readonly WorkflowInstanceManagementOrchestrationService orchestrationService;

    public WorkflowInstanceManagementOrchestrationServiceTests()
    {
        workflowInstanceManagementBrokerMock = new Mock<IWorkflowInstanceManagementBroker>(MockBehavior.Strict);
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(initialData: new Dictionary<string, string>
            {
                ["Services:Workflow"] = "https://workflow.test/",
                ["Settings:sslPort"] = "7157",
                ["Workflow:InstanceMaintenance:MaxAgeDays"] = "5",
                ["Workflow:QueueInstanceManagement:ExecutingTimeoutMinutes"] = "45",
            })
            .Build();

        orchestrationService = new WorkflowInstanceManagementOrchestrationService(
            workflowInstanceManagementBrokerMock.Object,
            Mock.Of<IServiceProvider>(),
            configuration,
            new WorkflowConfiguration(),
            NullLogger<WorkflowInstanceManagementOrchestrationService>.Instance);
    }

    [Fact]
    public async Task RunAsync_ShouldExecuteQueuedInstancesAndRequeueHungExecutingInstances()
    {
        // Given
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: []);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.FlushOldInstancesAsync(
cutoff: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 0);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.RequeueHungExecutingInstancesAsync(
cutoff: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        // When
        await orchestrationService.RunAsync();

        // Then
        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.RequeueHungExecutingInstancesAsync(
cutoff: It.Is<DateTimeOffset>(cutoff =>
                    cutoff < DateTimeOffset.UtcNow.AddMinutes(-44)
                    && cutoff > DateTimeOffset.UtcNow.AddMinutes(-46)),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.FlushOldInstancesAsync(
cutoff: It.Is<DateTimeOffset>(cutoff =>
                    cutoff < DateTimeOffset.UtcNow.AddDays(-4)
                    && cutoff > DateTimeOffset.UtcNow.AddDays(-6)),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.GetQueuedInstances(),
times: Times.Once);

        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void CreateWorkflowRequest_ShouldUseConfiguredWebSslPort()
    {
        // Given
        FlowInstanceData flowInstanceData = CreateQueuedFlowInstanceData();

        Token token = new()
        {
            Id = "workflow-token"
        };

        // When
        WorkflowRequest actualRequest = orchestrationService.CreateWorkflowRequest(dbInstance: flowInstanceData, token: token);

        // Then
        Assert.Equal(expected: $"https://{flowInstanceData.FlowDefinition.App.Domain}:7157/Api/", actual: actualRequest.Api);
        Assert.Equal(expected: flowInstanceData.FlowDefinition.Id, actual: actualRequest.FlowId);
        Assert.Equal(expected: flowInstanceData.Id, actual: actualRequest.InstanceId);
        Assert.Equal(expected: token.Id, actual: actualRequest.AuthToken);
    }

    [Fact]
    public async Task RunQueueInstanceManagementAsync_ShouldClaimQueuedInstances()
    {
        // Given
        FlowInstanceData queuedInstance = CreateQueuedFlowInstanceData();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: [queuedInstance]);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.ClaimQueuedInstanceAsync(
flowInstanceDataId: queuedInstance.Id,
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.RequeueHungExecutingInstancesAsync(
cutoff: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 0);

        // When
        await orchestrationService.RunQueueInstanceManagementAsync();

        // Then
        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.GetQueuedInstances(),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.ClaimQueuedInstanceAsync(
flowInstanceDataId: queuedInstance.Id,
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.RequeueHungExecutingInstancesAsync(
cutoff: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteWaitingQueuedInstanceByIdAsync_ShouldOnlyAttemptAtomicClaimForRequestedInstance()
    {
        // Given
        Guid instanceId = Guid.NewGuid();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.ClaimQueuedInstanceAsync(
flowInstanceDataId: instanceId,
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        // When
        await orchestrationService.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: instanceId);

        // Then
        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.ClaimQueuedInstanceAsync(flowInstanceDataId: instanceId, cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteWaitingQueuedInstanceByIdAsync_ShouldMarkClaimedInstanceFailedWhenExecutionThrows()
    {
        // Given
        FlowInstanceData queuedInstance = CreateQueuedFlowInstanceData();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.ClaimQueuedInstanceAsync(
flowInstanceDataId: queuedInstance.Id,
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: queuedInstance);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.MarkInstanceFailedAsync(
flowInstanceDataId: queuedInstance.Id,
failedAt: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        // When
        await orchestrationService.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedInstance.Id);

        // Then
        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.ClaimQueuedInstanceAsync(
flowInstanceDataId: queuedInstance.Id,
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.MarkInstanceFailedAsync(
flowInstanceDataId: queuedInstance.Id,
failedAt: It.Is<DateTimeOffset>(failedAt =>
                    failedAt > DateTimeOffset.UtcNow.AddMinutes(-1)
                    && failedAt <= DateTimeOffset.UtcNow.AddMinutes(1)),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    private static FlowInstanceData CreateQueuedFlowInstanceData() =>
        new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            State = "Queued",
            Start = DateTimeOffset.UtcNow,
            Caller = "system",
            FlowDefinition = new FlowDefinition
            {
                Id = Guid.NewGuid(),
                App = new App { Domain = "tenant.test" },
            },
        };
}