// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowCommunicationProcessingServiceTests
{
    [Fact]
    public async Task ShouldConnectWorkflowRequestAsync()
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.ConnectAsync(
                url: $"{request.Api}Hubs/Workflow"))
            .Returns(value: Task.CompletedTask);

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.SendAsync(
                level: "info",
                message: It.IsAny<string>(),
                instanceId: request.InstanceId.ToString()))
            .Returns(value: Task.CompletedTask);

        var service = CreateService();

        // When
        await service.ConnectWorkflowRequestAsync(workflowRequest: request);

        // Then
        workflowHubConnectionBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldLogWarningWhenWorkflowConnectionFailsAsync()
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.ConnectAsync(
                url: $"{request.Api}Hubs/Workflow"))
            .Throws(exception: new InvalidOperationException());

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.DisconnectAsync())
            .Returns(value: ValueTask.CompletedTask);

        var service = CreateService();

        // When
        await service.ConnectWorkflowRequestAsync(workflowRequest: request);

        // Then
        workflowHubConnectionBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDisconnectAndLogWhenWorkflowSendFailsAsync()
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.ConnectAsync(
                url: $"{request.Api}Hubs/Workflow"))
            .Returns(value: Task.CompletedTask);

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.SendAsync(
                level: "info",
                message: It.IsAny<string>(),
                instanceId: request.InstanceId.ToString()))
            .Throws(exception: new InvalidOperationException("send failed"));

        workflowHubConnectionBrokerMock
            .Setup(expression: broker => broker.DisconnectAsync())
            .Returns(value: ValueTask.CompletedTask);

        var service = CreateService();

        // When
        await service.ConnectWorkflowRequestAsync(workflowRequest: request);

        // Then
        workflowHubConnectionBrokerMock.VerifyAll();

        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == "send failed")),
            times: Times.Once());
    }
}