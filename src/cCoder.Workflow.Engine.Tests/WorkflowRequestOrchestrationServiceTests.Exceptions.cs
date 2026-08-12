// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings => new()
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
    public async Task ShouldMapExecuteWorkflowRequestAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        WorkflowRequest request = CreateWorkflowRequest();

        flowCommunicationProcessingServiceMock
            .Setup(expression: service => service
                .ConnectWorkflowRequestAsync(workflowRequest: request))
            .Throws(exception: exception);

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteWorkflowRequestAsync(workflowRequest: request);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}