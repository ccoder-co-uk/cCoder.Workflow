// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT009
public partial class WorkflowEventOrchestrationServiceTests
{
    [Fact]
    public void ShouldPrepareWorkflowEventDispatch()
    {
        // Given
        object payload = new { AppId = 7 };
        (int? AppId, string EventContext) expected = (7, "event");

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: "event",
                appIdOverride: 7))
            .Returns(value: expected);

        // When
        (int? AppId, string EventContext) actual = orchestrationService
            .PrepareWorkflowEventDispatch(payload, "event", 7);

        // Then
        actual.Should().Be(expected: expected);
        workflowEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeWorkflowEventPayload()
    {
        // Given
        object payload = new { Value = 1 };

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.SerializeWorkflowEventPayload(
                payload: payload))
            .Returns(value: "serialized");

        // When
        string actual = orchestrationService.SerializeWorkflowEventPayload(
            payload: payload);

        // Then
        actual.Should().Be(expected: "serialized");
        workflowEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldGetWorkflowEventSubscriptionsAsync()
    {
        // Given
        WorkflowEvent[] expected = [CreateRandomWorkflowEvent()];

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.GetSubscriptionsAsync(
                appId: 7,
                eventContext: "event"))
            .ReturnsAsync(value: expected);

        // When
        WorkflowEvent[] actual = await orchestrationService
            .GetWorkflowEventSubscriptionsAsync(appId: 7, eventContext: "event");

        // Then
        actual.Should().BeSameAs(expected: expected);
        workflowEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogWorkflowEventQueueFailureAsync()
    {
        // Given
        WorkflowEvent item = CreateRandomWorkflowEvent();
        Exception exception = new(message: "failed");

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.LogWorkflowEventQueueFailureAsync(
                workflowEvent: item,
                exception: exception))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.LogWorkflowEventQueueFailureAsync(
            workflowEvent: item,
            exception: exception);

        // Then
        workflowEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDelegateAddOrUpdateWorkflowEventsAsync()
    {
        // Given
        WorkflowEvent[] items = [CreateRandomWorkflowEvent()];
        Result<WorkflowEvent>[] expected = [new() { Success = true, Item = items[0] }];

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.AddOrUpdateWorkflowEvent(items))
            .ReturnsAsync(value: expected);

        // When
        IEnumerable<Result<WorkflowEvent>> actual = await orchestrationService
            .AddOrUpdateWorkflowEvent(items: items);

        // Then
        actual.Should().BeSameAs(expected: expected);
        workflowEventProcessingServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT009