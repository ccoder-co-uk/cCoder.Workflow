// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowCommunicationProcessingServiceTests
{
    [Fact]
    public async Task ShouldLogWorkflowRequestAsync()
    {
        // Given
        const string message = "message";
        WorkflowRequest request = CreateWorkflowRequest();
        var service = CreateService();

        // When
        await service.LogWorkflowRequestAsync(
            workflowRequest: request,
            level: WorkflowLogLevel.Info,
            message: message);

        await service.LogWorkflowRequestAsync(
            workflowRequest: request,
            level: WorkflowLogLevel.Error,
            message: message);

        await service.LogWorkflowRequestAsync(
            workflowRequest: request,
            level: WorkflowLogLevel.Fatal,
            message: message);

        // Then
        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == message)),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task ShouldTruncateLongWorkflowRequestLogAsync()
    {
        // Given
        string message = new(c: 'a', count: 4001);
        string loggedMessage = null;
        WorkflowRequest request = CreateWorkflowRequest();

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                message: "{Message}",
                args: It.IsAny<object[]>()))
            .Callback<string, object[]>(action: (_, arguments) =>
                loggedMessage = arguments.Single() as string);

        var service = CreateService();

        // When
        await service.LogWorkflowRequestAsync(
            workflowRequest: request,
            level: WorkflowLogLevel.Error,
            message: message);

        // Then
        loggedMessage.Length
            .Should()
            .BeLessThan(expected: message.Length);

        loggedMessage
            .Should()
            .Contain(expected: "characters cut");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldRejectInvalidWorkflowRequestAsync(string api)
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();
        request.Api = api;
        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .LogWorkflowRequestAsync(
                workflowRequest: request,
                level: WorkflowLogLevel.Info,
                message: "message");

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();

        loggingBrokerMock.VerifyNoOtherCalls();
    }
}