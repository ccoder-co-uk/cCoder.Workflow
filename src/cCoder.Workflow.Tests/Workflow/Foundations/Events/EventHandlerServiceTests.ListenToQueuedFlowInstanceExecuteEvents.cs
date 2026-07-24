// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public sealed class EventHandlerServiceQueuedFlowInstanceTests
{
    [Fact]
    public async Task ListenToQueuedFlowInstanceExecuteEvents_ShouldHandleQueuedAddAndUpdateEvents()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(MockBehavior.Strict);
        Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask> addHandler = null;
        Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask> updateHandler = null;

        eventHubBrokerMock
            .Setup(broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceManagementOrchestrationService>(
                "flow_instance_data_add",
                It.IsAny<Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>(
                (_, handler) => addHandler = handler);

        eventHubBrokerMock
            .Setup(broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceManagementOrchestrationService>(
                "flow_instance_data_update",
                It.IsAny<Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>(
                (_, handler) => updateHandler = handler);

        EventHandlerService service = new(eventHubBrokerMock.Object);
        Mock<IWorkflowInstanceManagementOrchestrationService> orchestrationServiceMock = new(MockBehavior.Strict);
        FlowInstanceData queuedAddInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData queuedUpdateInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData executingInstance = new() { Id = Guid.NewGuid(), State = "Executing" };

        orchestrationServiceMock
            .Setup(orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(queuedAddInstance.Id))
            .Returns(ValueTask.CompletedTask);

        orchestrationServiceMock
            .Setup(orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(queuedUpdateInstance.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        service.ListenToQueuedFlowInstanceExecuteEvents();
        await addHandler!(orchestrationServiceMock.Object, queuedAddInstance);
        await updateHandler!(orchestrationServiceMock.Object, queuedUpdateInstance);
        await updateHandler!(orchestrationServiceMock.Object, executingInstance);

        // Then
        orchestrationServiceMock.Verify(
            orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(queuedAddInstance.Id),
            Times.Once);

        orchestrationServiceMock.Verify(
            orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(queuedUpdateInstance.Id),
            Times.Once);

        orchestrationServiceMock.VerifyNoOtherCalls();
        eventHubBrokerMock.VerifyAll();
    }
}