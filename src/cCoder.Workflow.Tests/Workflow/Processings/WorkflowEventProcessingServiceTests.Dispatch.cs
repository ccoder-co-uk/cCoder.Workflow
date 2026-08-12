// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public void ShouldPrepareWorkflowEventDispatchFromPayload()
    {
        // Given
        var payload = new { AppId = 7, Path = "/home" };

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                "Workflow trigger event: AppId {AppId}, Context {EventContext}",
                It.IsAny<object[]>()));

        // When
        (int? AppId, string EventContext) result = workflowEventProcessingService
            .PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: "page_update");

        // Then
        result.AppId.Should().Be(expected: 7);
        result.EventContext.Should().Be(expected: "page_update/home");
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldPrepareWorkflowEventDispatchWithOverridesAndMissingPath()
    {
        // Given
        var payload = new { Name = "Payload" };

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                "Workflow trigger event: AppId {AppId}, Context {EventContext}",
                It.IsAny<object[]>()));

        // When
        (int? AppId, string EventContext) result = workflowEventProcessingService
            .PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: "event",
                appIdOverride: 9);

        // Then
        result.AppId.Should().Be(expected: 9);
        result.EventContext.Should().Be(expected: "event");
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeWorkflowEventPayload()
    {
        // Given
        object payload = new { Value = 1 };

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: payload))
            .Returns(value: "serialized");

        // When
        string result = workflowEventProcessingService
            .SerializeWorkflowEventPayload(payload: payload);

        // Then
        result.Should().Be(expected: "serialized");
        jsonBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogWorkflowEventQueueFailureAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        Exception exception = new(message: "Queue failed");

        loggingBrokerMock
            .Setup(expression: broker => broker.LogWarning(
                exception: exception,
                message: "Failed to queue a new workflow instance for subscription {SubscriptionId}, flow {FlowId}.",
                args: It.IsAny<object[]>()));

        // When
        await workflowEventProcessingService.LogWorkflowEventQueueFailureAsync(
            workflowEvent: workflowEvent,
            exception: exception);

        // Then
        loggingBrokerMock.VerifyAll();
    }
}