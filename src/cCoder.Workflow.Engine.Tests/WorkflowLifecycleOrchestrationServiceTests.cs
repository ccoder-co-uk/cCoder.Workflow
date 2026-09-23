// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowLifecycleOrchestrationServiceTests
{
    private readonly Mock<IFlowCommunicationService>
        flowCommunicationServiceMock = new();

    private readonly Mock<IFlowResultService> flowResultServiceMock = new();

    [Fact]
    public async Task StartFlowExecutionAsync_WhenRequested_ConnectsAndLogs()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();

        flowResultServiceMock
            .Setup(expression: service => service.Serialize(
                value: flowExecution.Request))
            .Returns(value: "request-json");

        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.StartFlowExecutionAsync(flowExecution: flowExecution);

        await flowExecution.Log(
            level: WorkflowLogLevel.Debug,
            message: "activity-log");

        // Then
        flowCommunicationServiceMock.Verify(expression: dependency =>
            dependency.ConnectWorkflowRequestAsync(
                workflowRequest: flowExecution.Request),
            times: Times.Once);

        flowCommunicationServiceMock.Verify(expression: dependency =>
            dependency.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: It.IsAny<WorkflowLogLevel>(),
                message: It.IsAny<string>()),
            times: Times.Exactly(callCount: 3));
    }

    [Fact]
    public async Task SaveFlowExecutionAsync_WhenRequested_SavesResult()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();
        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.SaveFlowExecutionAsync(flowExecution: flowExecution);

        // Then
        flowResultServiceMock.Verify(expression: dependency =>
            dependency.SaveFlowInstanceDataAsync(
                flowInstanceData: flowExecution.Result,
                apiRoot: flowExecution.Request.Api,
                authToken: flowExecution.Request.AuthToken),
            times: Times.Once);
    }

    [Fact]
    public async Task RecordFlowExecutionFailureAsync_WhenResultExists_PersistsFailure()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();
        WorkflowContext context = new() { ExecutionLog = [] };

        Exception failure = new(
            message: "outer",
            innerException: new Exception(message: "inner"));

        flowResultServiceMock
            .Setup(expression: service => service.Deserialize<WorkflowContext>(
                value: flowExecution.Result.ContextString))
            .Returns(value: context);

        flowResultServiceMock
            .Setup(expression: service => service.Serialize(value: context))
            .Returns(value: "failed-context");

        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.RecordFlowExecutionFailureAsync(
            flowExecution: flowExecution,
            exception: failure);

        // Then
        flowExecution.Result.State
            .Should()
            .Be(expected: "Failed");

        flowExecution.Result.ContextString
            .Should()
            .Be(expected: "failed-context");

        context.ExecutionLog
            .Should()
            .HaveCount(expected: 2);

        flowResultServiceMock.Verify(expression: dependency =>
            dependency.SaveFlowInstanceDataAsync(
                flowInstanceData: flowExecution.Result,
                apiRoot: flowExecution.Request.Api,
                authToken: flowExecution.Request.AuthToken),
            times: Times.Once);
    }

    [Fact]
    public async Task RecordFlowExecutionFailureAsync_WhenResultIsMissing_OnlyLogsFailure()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();
        flowExecution.Result = null;
        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.RecordFlowExecutionFailureAsync(
            flowExecution: flowExecution,
            exception: new Exception(message: "failed"));

        // Then
        flowResultServiceMock.Verify(expression: dependency =>
            dependency.SaveFlowInstanceDataAsync(
                flowInstanceData: It.IsAny<FlowInstanceData>(),
                apiRoot: It.IsAny<string>(),
                authToken: It.IsAny<string>()),
            times: Times.Never);

        flowCommunicationServiceMock.Verify(expression: dependency =>
            dependency.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: WorkflowLogLevel.Fatal,
                message: It.IsAny<string>()),
            times: Times.Once);
    }

    [Fact]
    public async Task RecordFlowExecutionFailureAsync_WhenPersistenceFails_LogsBothFailures()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();
        flowExecution.Result.ContextString = string.Empty;

        flowResultServiceMock
            .Setup(expression: service => service.Serialize(
                value: It.IsAny<WorkflowContext>()))
            .Returns(value: "failed-context");

        flowResultServiceMock
            .Setup(expression: dependency =>
                dependency.SaveFlowInstanceDataAsync(
                    flowInstanceData: It.IsAny<FlowInstanceData>(),
                    apiRoot: It.IsAny<string>(),
                    authToken: It.IsAny<string>()))
            .Throws(exception: new InvalidOperationException(
                message: "storage failed"));

        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.RecordFlowExecutionFailureAsync(
            flowExecution: flowExecution,
            exception: new Exception(message: "execution failed"));

        // Then
        flowCommunicationServiceMock.Verify(expression: dependency =>
            dependency.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: It.IsAny<WorkflowLogLevel>(),
                message: It.IsAny<string>()),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task RecordFlowExecutionFailureAsync_WhenContextIsInvalid_UsesEmptyContext()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();

        flowResultServiceMock
            .Setup(expression: service => service.Deserialize<WorkflowContext>(
                value: flowExecution.Result.ContextString))
            .Throws(exception: new InvalidOperationException(
                message: "invalid context"));

        flowResultServiceMock
            .Setup(expression: service => service.Serialize(
                value: It.IsAny<WorkflowContext>()))
            .Returns(value: "failed-context");

        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.RecordFlowExecutionFailureAsync(
            flowExecution: flowExecution,
            exception: new Exception(message: "failed"));

        // Then
        flowResultServiceMock.Verify(expression: service => service.Serialize(
            value: It.Is<WorkflowContext>(match: value =>
                value.ExecutionLog.Count == 1)),
            times: Times.Once);
    }

    [Fact]
    public async Task FinishFlowExecutionAsync_WhenRequested_LogsCompletion()
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();
        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        await service.FinishFlowExecutionAsync(flowExecution: flowExecution);

        // Then
        flowCommunicationServiceMock.Verify(expression: dependency =>
            dependency.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: WorkflowLogLevel.Info,
                message: "Done!"),
            times: Times.Once);
    }

    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task FinishFlowExecutionAsync_WhenDependencyFails_MapsException(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowExecution flowExecution = CreateFlowExecution();

        flowCommunicationServiceMock
            .Setup(expression: dependency =>
                dependency.LogWorkflowRequestAsync(
                    workflowRequest: flowExecution.Request,
                    level: WorkflowLogLevel.Info,
                    message: "Done!"))
            .Throws(exception: exception);

        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () =>
            await service.FinishFlowExecutionAsync(
                flowExecution: flowExecution);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public async Task FinishFlowExecutionAsync_WhenRequestIsMissing_ThrowsValidationException()
    {
        // Given
        WorkflowLifecycleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () =>
            await service.FinishFlowExecutionAsync(
                flowExecution: new FlowExecution());

        // Then
        await action
            .Should()
            .ThrowAsync<cCoder.Workflow.Engine.Models.Exceptions.WorkflowEngineValidationException>();
    }

    private WorkflowLifecycleOrchestrationService CreateService() =>
        new(
            flowCommunicationService: flowCommunicationServiceMock.Object,
            flowResultService: flowResultServiceMock.Object);

    private static FlowExecution CreateFlowExecution() =>
        new()
        {
            Request = new WorkflowRequest
            {
                InstanceId = Guid.NewGuid(),
                Api = "https://localhost/",
                AuthToken = "token"
            },
            Result = new FlowInstanceData
            {
                ContextString = "context",
                State = "Running"
            }
        };
}