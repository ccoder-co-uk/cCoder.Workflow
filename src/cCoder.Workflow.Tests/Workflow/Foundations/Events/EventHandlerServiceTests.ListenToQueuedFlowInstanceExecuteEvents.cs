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
            .Setup(expression: broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceManagementOrchestrationService>(
eventName: "flow_instance_data_add",
handler: It.IsAny<Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>(
action: (_, handler) => addHandler = handler);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceManagementOrchestrationService>(
eventName: "flow_instance_data_update",
handler: It.IsAny<Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceManagementOrchestrationService, FlowInstanceData, ValueTask>>(
action: (_, handler) => updateHandler = handler);

        EventHandlerService service = new(eventHubBrokerMock.Object);
        Mock<IWorkflowInstanceManagementOrchestrationService> orchestrationServiceMock = new(MockBehavior.Strict);
        FlowInstanceData queuedAddInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData queuedUpdateInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData executingInstance = new() { Id = Guid.NewGuid(), State = "Executing" };

        orchestrationServiceMock
            .Setup(expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedAddInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        orchestrationServiceMock
            .Setup(expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedUpdateInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        service.ListenToQueuedFlowInstanceExecuteEvents();
        await addHandler!(arg1: orchestrationServiceMock.Object, arg2: queuedAddInstance);
        await updateHandler!(arg1: orchestrationServiceMock.Object, arg2: queuedUpdateInstance);
        await updateHandler!(arg1: orchestrationServiceMock.Object, arg2: executingInstance);

        // Then
        orchestrationServiceMock.Verify(
expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedAddInstance.Id),
times: Times.Once);

        orchestrationServiceMock.Verify(
expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedUpdateInstance.Id),
times: Times.Once);

        orchestrationServiceMock.VerifyNoOtherCalls();
        eventHubBrokerMock.VerifyAll();
    }
}