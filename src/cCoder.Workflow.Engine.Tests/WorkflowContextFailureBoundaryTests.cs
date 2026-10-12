// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowContextFailureBoundaryTests
{
    [Theory]
    [InlineData("unreadable-original-context")]
    [InlineData("")]
    public async Task ShouldPreserveFailedContextAndReportThroughConnectedHubAsync(string originalContext)
    {
        // Given
        Mock<IWorkflowHubConnectionBroker> hub = new();
        Mock<IJsonBroker> json = new();
        Mock<IFlowResultService> results = new();

        json.Setup(expression: broker => broker.Deserialize<WorkflowContext>(value: originalContext))
            .Throws(exception: new InvalidOperationException(message: "Config setter failed"));

        results.Setup(expression: service => service.Deserialize<WorkflowContext>(value: originalContext))
            .Throws(exception: new InvalidOperationException(message: "Config setter failed"));

        results.Setup(expression: service => service.Serialize(value: It.IsAny<object>()))
            .Returns(value: "request-json");

        FlowCommunicationService communication = new(
            logger: Mock.Of<ILoggingBroker>(),
            workflowHubConnectionBroker: hub.Object);

        WorkflowLifecycleOrchestrationService lifecycle = new(
            flowCommunicationService: communication,
            flowResultService: results.Object);

        WorkflowRuntimeService runtime = new(
            workflowContextBroker: Mock.Of<IWorkflowContextBroker>(),
            jsonBroker: json.Object,
            reflectionBroker: Mock.Of<IReflectionBroker>());

        FlowExecution execution = new()
        {
            Request = new WorkflowRequest(
                api: "https://api.example/",
                token: "test-token",
                flowId: Guid.NewGuid(),
                instanceId: Guid.NewGuid()),
            Result = new FlowInstanceData { ContextString = originalContext }
        };

        // When
        await lifecycle.StartFlowExecutionAsync(flowExecution: execution);

        Exception failure = await Record.ExceptionAsync(testCode: async () =>
            await runtime.ExecuteFlowExecutionAsync(flowExecution: execution));

        await lifecycle.RecordFlowExecutionFailureAsync(
            flowExecution: execution,
            exception: failure);

        // Then
        Assert.NotNull(@object: failure);
        Assert.Equal(expected: originalContext, actual: execution.Result.ContextString);
        Assert.Equal(expected: "Failed", actual: execution.Result.State);

        hub.Verify(expression: broker => broker.ConnectAsync(url: "https://api.example/Hubs/Workflow"),
            times: Times.Once);

        hub.Verify(expression: broker => broker.SendAsync(
                level: "error",
                message: It.Is<string>(match: message => message.Contains(value: "Config setter failed")),
                instanceId: execution.Request.InstanceId.ToString()),
            times: Times.Once);

        hub.Verify(expression: broker => broker.SendAsync(
                level: "fatal",
                message: It.Is<string>(match: message => message.Contains(value: "Failed to process request")),
                instanceId: execution.Request.InstanceId.ToString()),
            times: Times.Once);

        results.Verify(expression: service => service.SaveFlowInstanceDataAsync(
                flowInstanceData: execution.Result,
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken),
            times: Times.Once);
    }
}