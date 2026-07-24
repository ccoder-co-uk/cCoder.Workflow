// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Processings;
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
        Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask> addHandler = null;
        Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask> updateHandler = null;

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceProcessingService>(
eventName: "flow_instance_data_add",
handler: It.IsAny<Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>>(
action: (_, handler) => addHandler = handler);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<FlowInstanceData, IWorkflowInstanceProcessingService>(
eventName: "flow_instance_data_update",
handler: It.IsAny<Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>>()))
            .Callback<string, Func<IWorkflowInstanceProcessingService, FlowInstanceData, ValueTask>>(
action: (_, handler) => updateHandler = handler);

        EventHandlerService service = new(eventHubBrokerMock.Object);
        Mock<IWorkflowInstanceProcessingService> processingServiceMock = new(MockBehavior.Strict);
        FlowInstanceData queuedAddInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData queuedUpdateInstance = new() { Id = Guid.NewGuid(), State = "Queued" };
        FlowInstanceData executingInstance = new() { Id = Guid.NewGuid(), State = "Executing" };

        processingServiceMock
            .Setup(expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedAddInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        processingServiceMock
            .Setup(expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedUpdateInstance.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        service.ListenToQueuedFlowInstanceExecuteEvents();
        await addHandler!(arg1: processingServiceMock.Object, arg2: queuedAddInstance);
        await updateHandler!(arg1: processingServiceMock.Object, arg2: queuedUpdateInstance);
        await updateHandler!(arg1: processingServiceMock.Object, arg2: executingInstance);

        // Then
        processingServiceMock.Verify(
expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedAddInstance.Id),
times: Times.Once);

        processingServiceMock.Verify(
expression: orchestration => orchestration.ExecuteWaitingQueuedInstanceByIdAsync(flowInstanceDataId: queuedUpdateInstance.Id),
times: Times.Once);

        processingServiceMock.VerifyNoOtherCalls();
        eventHubBrokerMock.VerifyAll();
    }
}