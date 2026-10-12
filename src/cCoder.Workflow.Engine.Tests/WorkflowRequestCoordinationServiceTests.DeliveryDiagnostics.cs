// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Coordinations;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestCoordinationServiceTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task ShouldFailDeliveryOnlyWhenWorkflowFailureCannotBePersistedAsync(bool persistenceFails, bool loggingFails)
    {
        // Given
        Mock<IFlowResultService> results = new();

        Mock<IFlowCommunicationService> communication = new();

        Mock<IFlowExecutionOrchestrationService> execution = new();

        FlowInstanceData instance = new() { ContextString = "original-context" };

        execution.Setup(expression: service => service.ExecuteFlowExecutionAsync(flowExecution: It.IsAny<FlowExecution>()))
            .Callback<FlowExecution>(action: flow => flow.Result = instance)
            .Throws(exception: new InvalidOperationException(message: "workflow evaluation failed"));

        results.Setup(expression: service => service.Deserialize<WorkflowContext>(value: "original-context"))
            .Throws(exception: new InvalidOperationException(message: "context is unreadable"));

        if (persistenceFails)
        {
            results.Setup(expression: service => service.SaveFlowInstanceDataAsync(
                    flowInstanceData: instance,
                    apiRoot: It.IsAny<string>(),
                    authToken: It.IsAny<string>()))
                .Throws(exception: new InvalidOperationException(message: "API unavailable"));

        }

        if (loggingFails)
        {
            communication.Setup(expression: service => service.LogWorkflowRequestAsync(
                    workflowRequest: It.IsAny<WorkflowRequest>(),
                    level: WorkflowLogLevel.Fatal,
                    message: It.IsAny<string>()))
                .Throws(exception: new InvalidOperationException(message: "logging unavailable"));

        }

        WorkflowLifecycleOrchestrationService lifecycle = new(
            flowCommunicationService: communication.Object,
            flowResultService: results.Object);

        WorkflowRequestCoordinationService coordinator = new(
            flowExecutionOrchestrationService: execution.Object,
            workflowLifecycleOrchestrationService: lifecycle);

        WorkflowRequest request = new(
            api: "https://api.example/",
            token: "test-token",
            flowId: Guid.NewGuid(),
            instanceId: Guid.NewGuid());

        // When
        Exception failure = await Record.ExceptionAsync(testCode: async () =>
            await coordinator.ExecuteWorkflowRequestAsync(workflowRequest: request));

        // Then
        results.Verify(expression: service => service.SaveFlowInstanceDataAsync(
                flowInstanceData: instance,
                apiRoot: request.Api,
                authToken: request.AuthToken),
            times: Times.Once);

        communication.Verify(expression: service => service.LogWorkflowRequestAsync(
                workflowRequest: request,
                level: WorkflowLogLevel.Fatal,
                message: It.IsAny<string>()),
            times: Times.Once);

        Assert.Equal(expected: "original-context", actual: instance.ContextString);

        if (persistenceFails || loggingFails)
        {
            Assert.NotNull(@object: failure);

            Assert.Contains(expectedSubstring: "workflow evaluation failed", actualString: failure.ToString());

            if (persistenceFails)
            {
                Assert.Contains(expectedSubstring: "API unavailable", actualString: failure.ToString());

            }

            if (loggingFails)
            {
                Assert.Contains(expectedSubstring: "logging unavailable", actualString: failure.ToString());

            }
        }
        else
        {
            Assert.Null(@object: failure);

        }
    }

    [Fact]
    public async Task ShouldSettleConfirmedConflictWithoutFailureOrCompletionLoggingAsync()
    {
        // Given
        Mock<IFlowExecutionOrchestrationService> execution = new();

        Mock<IWorkflowLifecycleOrchestrationService> lifecycle = new();

        execution.Setup(expression: service => service.ExecuteFlowExecutionAsync(flowExecution: It.IsAny<FlowExecution>()))
            .Throws(exception: new InvalidOperationException(message: "wrapped", innerException: new WorkflowAttemptConflictException()));

        lifecycle.Setup(expression: service => service.FinishFlowExecutionAsync(flowExecution: It.IsAny<FlowExecution>()))
            .Throws(exception: new InvalidOperationException(message: "logging unavailable"));

        WorkflowRequestCoordinationService coordinator = new(
            flowExecutionOrchestrationService: execution.Object, workflowLifecycleOrchestrationService: lifecycle.Object);

        // When
        await coordinator.ExecuteWorkflowRequestAsync(workflowRequest: new WorkflowRequest(
            api: "https://trusted.example/Api/", token: "token", flowId: Guid.NewGuid(), instanceId: Guid.NewGuid()));

        // Then
        lifecycle.Verify(expression: service => service.RecordFlowExecutionFailureAsync(
            flowExecution: It.IsAny<FlowExecution>(), exception: It.IsAny<Exception>()), times: Times.Never);

        lifecycle.Verify(expression: service => service.FinishFlowExecutionAsync(flowExecution: It.IsAny<FlowExecution>()), times: Times.Never);

    }

}