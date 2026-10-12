// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Models.Entities;
using cCoder.Security.Exposures;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009
public sealed partial class WorkflowInstanceProcessingServiceTests
{
    private readonly Mock<IWorkflowInstanceManagementBroker> workflowInstanceManagementBrokerMock;
    private readonly Mock<IFlowInstanceDataBroker> flowInstanceDataBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly Mock<ITokenManager> tokenManagerMock;
    private readonly Mock<IWorkflowExecutionEventBroker> workflowExecutionEventBrokerMock;
    private readonly WorkflowConfiguration configuration;
    private readonly WorkflowInstanceAggregationService processingService;

    public WorkflowInstanceProcessingServiceTests()
    {
        workflowInstanceManagementBrokerMock = new(
            behavior: MockBehavior.Strict);

        flowInstanceDataBrokerMock = new(
            behavior: MockBehavior.Strict);
        loggingBrokerMock = new();
        tokenManagerMock = new();
        workflowExecutionEventBrokerMock = new();
        configuration = new()
        {
            ServiceUrl = "https://workflow.test/",
            SslPort = 7157,
            InstanceMaintenance = new WorkflowInstanceMaintenanceConfiguration
            {
                MaxAgeDays = 5
            },
            QueueInstanceManagement = new()
            {
                ExecutingTimeoutMinutes = 45
            }
        };

        IWorkflowInstanceManagementService managementService =
            new WorkflowInstanceManagementService(
                workflowInstanceManagementBroker: workflowInstanceManagementBrokerMock.Object,
                jsonBroker: new cCoder.Workflow.Brokers.JsonBroker());

        IWorkflowConfigurationService configurationService =
            new WorkflowConfigurationService(
                configurationBroker: new WorkflowConfigurationBroker(
                    configuration: configuration));

        IWorkflowTokenService tokenService =
            new WorkflowTokenService(
                workflowTokenBroker: new WorkflowTokenBroker(
                    tokenManager: tokenManagerMock.Object));

        IWorkflowExecutionEventService executionEventService =
            new WorkflowExecutionEventService(
                workflowExecutionEventBroker: workflowExecutionEventBrokerMock.Object);

        processingService = new WorkflowInstanceAggregationService(
            workflowInstanceManagementService: managementService,
            flowInstanceDataService: new FlowInstanceDataService(
                flowInstanceDataBroker: flowInstanceDataBrokerMock.Object,
                authorizationBroker: Mock.Of<IAuthorizationBroker>(),
                oDataResultBroker: Mock.Of<cCoder.Workflow.Brokers.OData.IODataResultBroker>(),
                jsonBroker: new cCoder.Workflow.Brokers.JsonBroker()),
            workflowTokenService: tokenService,
            workflowExecutionEventService: executionEventService,
            workflowConfigurationService: configurationService,
            loggingBroker: loggingBrokerMock.Object,
                jsonBroker: new cCoder.Workflow.Brokers.JsonBroker());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetAll_ShouldDelegateToFlowInstanceDataManager(bool ignoreFilters)
    {
        // Given
        IQueryable<FlowInstanceData> expected =
            new[] { CreateQueuedFlowInstanceData() }.AsQueryable();

        if (ignoreFilters)
        {
            flowInstanceDataBrokerMock
                .Setup(expression: broker => broker.SelectAllFlowInstanceDataIgnoringQueryFilters())
                .Returns(value: expected);
        }
        else
        {
            flowInstanceDataBrokerMock
                .Setup(expression: broker => broker.SelectAllFlowInstanceData())
                .Returns(value: expected);
        }

        // When
        IQueryable<FlowInstanceData> actual =
            processingService.GetAll(ignoreFilters: ignoreFilters);

        // Then
        Assert.Same(expected: expected, actual: actual);

        flowInstanceDataBrokerMock.VerifyAll();
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RunAsync_ShouldExecuteQueuedInstancesAndDropOldInstances()
    {
        // Given
        SetupNoExpiredInstances();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: []);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.FlushOldInstancesAsync(
cutoff: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 0);

        // When
        await processingService.RunAsync();

        // Then
        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.FlushOldInstancesAsync(
cutoff: It.Is<DateTimeOffset>(match: cutoff =>
                    cutoff < DateTimeOffset.UtcNow.AddDays(days: -4)
                    && cutoff > DateTimeOffset.UtcNow.AddDays(days: -6)),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
expression: broker => broker.GetQueuedInstances(),
times: Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
            expression: broker => broker.SelectExpiredInstances(cutoff: It.IsAny<DateTimeOffset>()),
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
        WorkflowRequest actualRequest = processingService.CreateWorkflowRequest(dbInstance: flowInstanceData, token: token);

        // Then
        Assert.Equal(expected: $"https://{flowInstanceData.FlowDefinition.App.Domain}:7157/Api/", actual: actualRequest.Api);
        Assert.Equal(expected: flowInstanceData.FlowDefinition.Id, actual: actualRequest.FlowId);
        Assert.Equal(expected: flowInstanceData.Id, actual: actualRequest.InstanceId);
        Assert.Equal(expected: token.Id, actual: actualRequest.AuthToken);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(443)]
    public void CreateWorkflowRequest_ShouldOmitUnsetOrDefaultWebSslPort(int sslPort)
    {
        // Given
        configuration.SslPort = sslPort;
        FlowInstanceData flowInstanceData = CreateQueuedFlowInstanceData();

        Token token = new()
        {
            Id = "workflow-token"
        };

        // When
        WorkflowRequest actualRequest = processingService.CreateWorkflowRequest(dbInstance: flowInstanceData, token: token);

        // Then
        Assert.Equal(expected: $"https://{flowInstanceData.FlowDefinition.App.Domain}/Api/", actual: actualRequest.Api);
    }

    [Fact]
    public async Task RunQueueInstanceBackgroundServiceDependencyAsync_ShouldIgnoreInstanceRemovedBeforeClaim()
    {
        // Given
        FlowInstanceData queuedInstance = CreateQueuedFlowInstanceData();
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.GetQueuedInstances())
            .Returns(value: [queuedInstance]);
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: queuedInstance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        // When
        await processingService.RunQueueInstanceBackgroundServiceDependencyAsync();

        // Then
        workflowInstanceManagementBrokerMock.Verify(
            expression: broker => broker.GetQueuedInstances(), times: Times.Once);
        workflowInstanceManagementBrokerMock.Verify(
            expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: queuedInstance.Id,
                cancellationToken: It.IsAny<CancellationToken>()), times: Times.Once);
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
        tokenManagerMock.VerifyNoOtherCalls();
        workflowExecutionEventBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteWaitingQueuedInstanceByIdAsync_ShouldOnlyLoadRequestedInstance()
    {
        // Given
        Guid instanceId = Guid.NewGuid();
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: instanceId);

        // Then
        workflowInstanceManagementBrokerMock.Verify(
            expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: It.IsAny<CancellationToken>()), times: Times.Once);
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteWaitingQueuedInstanceByIdAsync_ShouldLogWhenDispatchThrows()
    {
        // Given
        FlowInstanceData queuedInstance = CreateQueuedFlowInstanceData();
        SetupQueuedClaim(instance: queuedInstance);

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedInstance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        Assert.Equal(expected: "Failed", actual: queuedInstance.State);
    }

    private void SetupNoExpiredInstances() =>
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectExpiredInstances(cutoff: It.IsAny<DateTimeOffset>()))
            .Returns(value: []);

    private void SetupQueuedClaim(FlowInstanceData instance)
    {
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);
        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.Is<WorkflowInstanceExchange>(match: exchange =>
                    exchange.Expected.Id == instance.Id && exchange.Replacement.Id == instance.Id),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Callback<WorkflowInstanceExchange, CancellationToken>(action: (exchange, cancellationToken) =>
            {
                instance.ContextJson = exchange.Replacement.ContextJson;
                instance.State = exchange.Replacement.State;
                instance.Start = exchange.Replacement.Start;
                instance.End = exchange.Replacement.End;
            })
            .ReturnsAsync(value: 1);
    }

    private static FlowInstanceData CreateQueuedFlowInstanceData() =>
        new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            State = "Queued",
            ContextString = "{}",
            Start = DateTimeOffset.UtcNow,
            Caller = "system",
            FlowDefinition = new FlowDefinition
            {
                Id = Guid.NewGuid(),
                App = new App { Domain = "tenant.test" },
            },
        };

}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009