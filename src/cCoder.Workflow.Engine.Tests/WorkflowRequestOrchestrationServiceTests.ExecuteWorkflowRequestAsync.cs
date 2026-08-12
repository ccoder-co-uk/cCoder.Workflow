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

        flowCommunicationProcessingServiceMock
            .Setup(expression: service => service
                .ConnectWorkflowRequestAsync(workflowRequest: request))
            .Returns(value: ValueTask.CompletedTask);

        flowCommunicationProcessingServiceMock
            .Setup(expression: service => service.LogWorkflowRequestAsync(
                workflowRequest: request,
                level: It.IsAny<WorkflowLogLevel>(),
                message: It.IsAny<string>()))
            .Returns(value: ValueTask.CompletedTask);

        flowInstanceProcessingServiceMock
            .Setup(expression: service => service.ExecuteFlowExecutionAsync(
                flowExecution: It.IsAny<FlowExecution>()))
            .Callback<FlowExecution>(action: execution =>
                capturedExecution = execution)
            .Returns<FlowExecution>(valueFunction: execution =>
                ValueTask.FromResult(
                    result: CompleteExecution(execution: execution)));

        flowResultProcessingServiceMock
            .Setup(expression: service => service.SaveFlowInstanceDataAsync(
                flowInstanceData:
                    It.IsAny<cCoder.Data.Models.Workflow.FlowInstanceData>(),
                apiRoot: request.Api,
                authToken: request.AuthToken))
            .Returns(value: ValueTask.CompletedTask);

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

        capturedExecution.Log
            .Should()
            .NotBeNull();

        flowCommunicationProcessingServiceMock.Verify(
            expression: dependency => dependency.LogWorkflowRequestAsync(
                workflowRequest: request,
                level: It.IsAny<WorkflowLogLevel>(),
                message: It.IsAny<string>()),
            times: Times.Exactly(callCount: 3));

        flowCommunicationProcessingServiceMock.Verify(
            expression: dependency => dependency
                .ConnectWorkflowRequestAsync(workflowRequest: request),
            times: Times.Once());

        flowCommunicationProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceProcessingServiceMock.VerifyAll();
        flowResultProcessingServiceMock.VerifyAll();
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

        flowCommunicationProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceProcessingServiceMock.VerifyNoOtherCalls();
        flowResultProcessingServiceMock.VerifyNoOtherCalls();
    }
}