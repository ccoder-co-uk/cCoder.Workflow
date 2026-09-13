// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    [Fact]
    public void ShouldPrepareDispatchFromPayload()
    {
        // Given
        var payload = new { AppId = 7, Path = "/home" };

        loggingBrokerMock.Setup(expression: broker => broker.LogDebug(
            message: "Workflow trigger event: AppId {AppId}, Context {EventContext}",
            args: new object[] { 7, "page_update/home" }));

        // When
        (int? AppId, string EventContext) result = workflowEventService.PrepareDispatch(
            payload: payload,
            eventName: "page_update");

        // Then
        result
            .Should()
            .Be(expected: (7, "page_update/home"));

        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldPrepareDispatchFromOverrideWithoutPath()
    {
        // Given
        var payload = new { Name = "Payload" };

        loggingBrokerMock.Setup(expression: broker => broker.LogDebug(
            message: "Workflow trigger event: AppId {AppId}, Context {EventContext}",
            args: new object[] { 9, "event" }));

        // When
        (int? AppId, string EventContext) result = workflowEventService.PrepareDispatch(
            payload: payload,
            eventName: "event",
            appIdOverride: 9);

        // Then
        result
            .Should()
            .Be(expected: (9, "event"));

        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializePayload()
    {
        // Given
        object payload = new { Value = 1 };

        jsonBrokerMock.Setup(expression: broker => broker.Serialize(value: payload))
            .Returns(value: "serialized");

        // When
        string result = workflowEventService.SerializePayload(payload: payload);

        // Then
        result
            .Should()
            .Be(expected: "serialized");

        jsonBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogSubscriberCount()
    {
        // Given
        loggingBrokerMock.Setup(expression: broker => broker.LogDebug(
            message: "Found {Count} subscribers, calling ...",
            args: 3));

        // When
        bool result = workflowEventService.LogWorkflowEventSubscriptionsFound(count: 3);

        // Then
        result
            .Should()
            .BeTrue();

        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldLogQueueFailure()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        Exception exception = new(message: "Queue failed");

        loggingBrokerMock.Setup(expression: broker => broker.LogWarning(
            exception: exception,
            message: "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
            args: new object[] { workflowEvent.Id, workflowEvent.FlowId }));

        // When
        bool result = workflowEventService.LogWorkflowEventQueueFailure(
            workflowEvent: workflowEvent,
            exception: exception);

        // Then
        result
            .Should()
            .BeTrue();

        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldGetSubscriptions()
    {
        // Given
        WorkflowEvent[] expected = [CreateRandomWorkflowEvent()];

        workflowEventBrokerMock.Setup(expression: broker => broker.SelectWorkflowEventSubscriptions(
            appId: 5,
            eventContext: "page_update/home"))
            .Returns(value: expected);

        // When
        WorkflowEvent[] result = workflowEventService.GetSubscriptions(
            appId: 5,
            eventContext: "page_update/home");

        // Then
        result
            .Should()
            .BeSameAs(expected: expected);

        workflowEventBrokerMock.VerifyAll();
    }
}