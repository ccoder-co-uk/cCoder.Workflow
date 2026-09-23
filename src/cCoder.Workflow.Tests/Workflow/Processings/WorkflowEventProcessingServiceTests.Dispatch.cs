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

        workflowEventServiceMock
            .Setup(expression: service => service.PrepareDispatch(
                payload: payload,
                eventName: "page_update",
                appIdOverride: null))
            .Returns(value: (7, "page_update/home"));

        // When
        (int? AppId, string EventContext) result = workflowEventProcessingService
            .PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: "page_update");

        // Then
        result.AppId.Should()
            .Be(expected: 7);

        result.EventContext.Should()
            .Be(expected: "page_update/home");

        workflowEventServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldPrepareWorkflowEventDispatchWithOverridesAndMissingPath()
    {
        // Given
        var payload = new { Name = "Payload" };

        workflowEventServiceMock
            .Setup(expression: service => service.PrepareDispatch(
                payload: payload,
                eventName: "event",
                appIdOverride: 9))
            .Returns(value: (9, "event"));

        // When
        (int? AppId, string EventContext) result = workflowEventProcessingService
            .PrepareWorkflowEventDispatch(
                payload: payload,
                eventName: "event",
                appIdOverride: 9);

        // Then
        result.AppId.Should()
            .Be(expected: 9);

        result.EventContext.Should()
            .Be(expected: "event");

        workflowEventServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeWorkflowEventPayload()
    {
        // Given
        object payload = new { Value = 1 };

        workflowEventServiceMock
            .Setup(expression: service => service.SerializePayload(
                payload: payload))
            .Returns(value: "serialized");

        // When
        string result = workflowEventProcessingService
            .SerializeWorkflowEventPayload(payload: payload);

        // Then
        result.Should()
            .Be(expected: "serialized");

        workflowEventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogWorkflowEventQueueFailureAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        Exception exception = new(message: "Queue failed");

        workflowEventServiceMock
            .Setup(expression: service => service.LogWorkflowEventQueueFailure(
                workflowEvent: workflowEvent,
                exception: exception))
            .Returns(value: true);

        // When
        await workflowEventProcessingService.LogWorkflowEventQueueFailureAsync(
            workflowEvent: workflowEvent,
            exception: exception);

        // Then
        workflowEventServiceMock.VerifyAll();
    }
}