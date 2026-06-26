using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
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
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Services:Workflow"] = "https://workflow.test/",
                ["Settings:sslPort"] = "443",
            })
            .Build();

        orchestrationService = new WorkflowInstanceManagementOrchestrationService(
            workflowInstanceManagementBrokerMock.Object,
            Mock.Of<IServiceProvider>(),
            configuration,
            NullLogger<WorkflowInstanceManagementOrchestrationService>.Instance);
    }

    [Fact]
    public async Task RunAsync_ShouldRequeueHungExecutingInstancesBeforeClaimingAllQueuedInstances()
    {
        // Given
        FlowInstanceData firstQueuedInstance = CreateQueuedFlowInstanceData();
        FlowInstanceData secondQueuedInstance = CreateQueuedFlowInstanceData();

        workflowInstanceManagementBrokerMock
            .Setup(broker => broker.FlushOldInstancesAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        workflowInstanceManagementBrokerMock
            .Setup(broker => broker.RequeueHungExecutingInstancesAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        workflowInstanceManagementBrokerMock
            .Setup(broker => broker.GetQueuedInstances())
            .Returns([firstQueuedInstance, secondQueuedInstance]);

        workflowInstanceManagementBrokerMock
            .Setup(broker => broker.ClaimQueuedInstanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((FlowInstanceData)null);

        // When
        await orchestrationService.RunAsync();

        // Then
        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.RequeueHungExecutingInstancesAsync(
                It.Is<DateTimeOffset>(cutoff => cutoff < DateTimeOffset.UtcNow.AddMinutes(-29)),
                It.IsAny<CancellationToken>()),
            Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.FlushOldInstancesAsync(
                It.Is<DateTimeOffset>(cutoff => cutoff < DateTimeOffset.UtcNow.AddDays(-6)),
                It.IsAny<CancellationToken>()),
            Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.GetQueuedInstances(),
            Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.ClaimQueuedInstanceAsync(firstQueuedInstance.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.ClaimQueuedInstanceAsync(secondQueuedInstance.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteWaitingQueuedInstanceByIdAsync_ShouldOnlyAttemptAtomicClaimForRequestedInstance()
    {
        // Given
        Guid instanceId = Guid.NewGuid();

        workflowInstanceManagementBrokerMock
            .Setup(broker => broker.ClaimQueuedInstanceAsync(
                instanceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((FlowInstanceData)null);

        // When
        await orchestrationService.ExecuteWaitingQueuedInstanceByIdAsync(instanceId);

        // Then
        workflowInstanceManagementBrokerMock.Verify(
            broker => broker.ClaimQueuedInstanceAsync(instanceId, It.IsAny<CancellationToken>()),
            Times.Once);

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
