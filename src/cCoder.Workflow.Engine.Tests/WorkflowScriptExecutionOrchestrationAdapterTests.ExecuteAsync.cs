// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionOrchestrationAdapterTests
{
    [Fact]
    public async Task ShouldExecuteAsync()
    {
        // Given
        const string payload = "return true";
        const string expected = "true";

        Mock<IWorkflowScriptExecutionProcessingService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service
                .ExecuteWorkflowScriptAsync(
                    payload: payload,
                    useDetails: true))
            .Returns(value: ValueTask.FromResult(result: expected));

        WorkflowScriptExecutionOrchestrationAdapter adapter =
            new(workflowScriptExecutionProcessingService: serviceMock.Object);

        // When
        string actual = await adapter.ExecuteAsync(
            payload: payload,
            useDetails: true);

        // Then
        actual
            .Should()
            .Be(expected: expected);

        serviceMock.VerifyAll();
    }
}