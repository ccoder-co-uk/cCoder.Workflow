// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using cCoder.Workflow.Engine.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRuntimeServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new WorkflowEngineValidationException(
                    innerException: new Exception()),
                typeof(WorkflowEngineValidationException)
            },
            {
                new WorkflowEngineDependencyException(
                    innerException: new Exception()),
                typeof(WorkflowEngineDependencyException)
            },
            {
                new ValidationException(),
                typeof(WorkflowEngineValidationException)
            },
            {
                new InvalidOperationException(),
                typeof(WorkflowEngineDependencyException)
            },
            {
                new Exception(),
                typeof(WorkflowEngineServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRuntimeFailureAsync(
        Exception exception,
        Type expectedType)
    {
        // Given
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        jsonBrokerMock
            .Setup(expression: broker =>
                broker.Deserialize<WorkflowContext>(value: "context"))
            .Throws(exception: exception);

        FlowExecution flowExecution = new()
        {
            Request = new(
                api: "https://localhost/",
                token: "token",
                flowId: Guid.NewGuid(),
                instanceId: Guid.NewGuid()),
            Result = new FlowInstanceData
            {
                ContextString = "context"
            },
            Log = (_, _) => Task.CompletedTask
        };

        var service = new WorkflowRuntimeService(
            workflowContextBroker:
                Mock.Of<IWorkflowContextBroker>(),
            jsonBroker: jsonBrokerMock.Object,
            reflectionBroker: Mock.Of<IReflectionBroker>());

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: flowExecution);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}