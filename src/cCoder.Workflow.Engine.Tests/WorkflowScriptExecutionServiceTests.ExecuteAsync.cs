// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_DelegatesToWorkflowScriptExecutionOrchestrationService()
    {
        // Given
        const string payload = "return true";
        const string expectedResult = "true";

        processingServiceMock
            .Setup(expression: service => service.ExecuteWorkflowScriptAsync(payload: payload, useDetails: true))
            .Returns(value: new ValueTask<string>(result: expectedResult));

        // When
        string actualResult = await workflowScriptExecutionService.ExecuteAsync(payload: payload, useDetails: true);

        // Then
        actualResult.Should()
            .Be(expected: expectedResult);

        processingServiceMock.Verify(
            expression: service => service.ExecuteWorkflowScriptAsync(payload: payload, useDetails: true),
            times: Times.Once);
    }
}