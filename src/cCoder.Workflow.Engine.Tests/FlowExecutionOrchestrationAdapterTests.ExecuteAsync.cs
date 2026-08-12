// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowExecutionOrchestrationAdapterTests
{
    [Fact]
    public async Task ShouldExecuteAsync()
    {
        // Given
        WorkflowRequest request = new(
            api: "https://localhost/",
            token: "token",
            flowId: Guid.NewGuid(),
            instanceId: Guid.NewGuid());

        Mock<IWorkflowRequestOrchestrationService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service
                .ExecuteWorkflowRequestAsync(workflowRequest: request))
            .Returns(value: ValueTask.CompletedTask);

        FlowExecutionOrchestrationAdapter adapter =
            new(workflowRequestOrchestrationService: serviceMock.Object);

        // When
        await adapter.ExecuteAsync(request: request);

        // Then
        serviceMock.VerifyAll();
    }
}