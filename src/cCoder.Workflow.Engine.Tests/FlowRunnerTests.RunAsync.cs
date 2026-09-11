// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowRunnerTests
{
    [Fact]
    public async Task RunAsync_DelegatesToFlowExecutionOrchestrationService()
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();

        // When
        await flowRunner.RunAsync(workflowRequest: request);

        // Then
        flowExecutionOrchestrationServiceMock.Verify(expression: service => service.ExecuteAsync(workflowRequest: request), times: Times.Once);
    }
}