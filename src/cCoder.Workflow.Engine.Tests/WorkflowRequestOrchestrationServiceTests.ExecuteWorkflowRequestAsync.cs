// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestOrchestrationServiceTests
{
    public static TheoryData<WorkflowRequest> InvalidWorkflowRequests => new()
    {
        null,
        new WorkflowRequest
        {
            Api = "https://localhost/",
            InstanceId = Guid.Empty
        },
        new WorkflowRequest
        {
            Api = " ",
            InstanceId = Guid.NewGuid()
        }
    };

    [Fact]
    public async Task ShouldExecuteWorkflowRequestAsync()
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();
        FlowExecution capturedExecution = null;

        workflowLifecycleOrchestrationServiceMock
            .Setup(expression: service => service
                .StartFlowExecutionAsync(
                    flowExecution: It.IsAny<FlowExecution>()))
            .Returns(value: ValueTask.CompletedTask);

        workflowLifecycleOrchestrationServiceMock
            .Setup(expression: service => service
                .SaveFlowExecutionAsync(
                    flowExecution: It.IsAny<FlowExecution>()))
            .Returns(value: ValueTask.CompletedTask);

        workflowLifecycleOrchestrationServiceMock
            .Setup(expression: service => service
                .FinishFlowExecutionAsync(
                    flowExecution: It.IsAny<FlowExecution>()))
            .Returns(value: ValueTask.CompletedTask);

        flowInstanceProcessingServiceMock
            .Setup(expression: service => service.ExecuteFlowExecutionAsync(
                flowExecution: It.IsAny<FlowExecution>()))
            .Callback<FlowExecution>(action: execution =>
                capturedExecution = execution)
            .Returns<FlowExecution>(valueFunction: execution =>
                ValueTask.FromResult(
                    result: CompleteExecution(execution: execution)));

        var service = CreateService();

        // When
        await service.ExecuteWorkflowRequestAsync(workflowRequest: request);

        // Then
        capturedExecution
            .Should()
            .NotBeNull();

        capturedExecution.Request
            .Should()
            .BeSameAs(expected: request);

        workflowLifecycleOrchestrationServiceMock.VerifyAll();
        flowInstanceProcessingServiceMock.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(InvalidWorkflowRequests))]
    public async Task ShouldRejectInvalidWorkflowRequestAsync(
        WorkflowRequest request)
    {
        // Given
        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteWorkflowRequestAsync(workflowRequest: request);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();

        workflowLifecycleOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceProcessingServiceMock.VerifyNoOtherCalls();
    }
}