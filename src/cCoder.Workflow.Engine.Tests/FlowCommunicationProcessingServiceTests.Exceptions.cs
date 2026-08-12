// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowCommunicationProcessingServiceTests
{
    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task ShouldMapLogWorkflowRequestAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        const string message = "message";
        WorkflowRequest request = CreateWorkflowRequest();

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                message: "{Message}",
                args: It.IsAny<object[]>()))
            .Throws(exception: exception);

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .LogWorkflowRequestAsync(
                workflowRequest: request,
                level: WorkflowLogLevel.Error,
                message: message);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}