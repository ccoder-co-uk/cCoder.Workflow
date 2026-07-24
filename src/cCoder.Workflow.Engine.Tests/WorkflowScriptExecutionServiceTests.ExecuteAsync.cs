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
        orchestrationServiceMock
            .Setup(expression: service => service.ExecuteAsync(payload: payload, useDetails: true))
            .ReturnsAsync(value: expectedResult);

        // When
        string actualResult = await workflowScriptExecutionService.ExecuteAsync(payload: payload, useDetails: true);

        // Then
        actualResult.Should()
            .Be(expected: expectedResult);
        orchestrationServiceMock.Verify(expression: service => service.ExecuteAsync(payload: payload, useDetails: true), times: Times.Once);
    }
}