// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionProcessingServiceTests
{
    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task ShouldMapExecuteWorkflowScriptAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        const string payload = "return value";

        scriptBrokerMock
            .Setup(expression: broker => broker.Run<object>(
                code: payload,
                imports: It.IsAny<string[]>(),
                args: null,
                log: It.IsAny<Action<WorkflowLogLevel, string>>()))
            .Throws(exception: exception);

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteWorkflowScriptAsync(payload: payload, useDetails: false);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}