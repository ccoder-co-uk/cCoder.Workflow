// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowInstanceProcessingServiceTests
{
    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task ShouldMapExecuteFlowExecutionAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowExecution execution = CreateFlowExecution();

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.GetStringAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>()))
            .Throws(exception: exception);

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: execution);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}