// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Workflow.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
    public static TheoryData<Exception, Type> DependencyExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), typeof(WorkflowValidationException) },
        { new WorkflowDependencyException(innerException: new Exception()), typeof(WorkflowDependencyException) },
        { new ValidationException(), typeof(WorkflowValidationException) },
        { new InvalidOperationException(), typeof(WorkflowDependencyException) },
        { new SecurityException(), typeof(SecurityException) },
        { new Exception(), typeof(WorkflowServiceException) }
    };

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task QueueAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        Guid flowDefinitionId = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString();
        string args = "{}";

        flowQueueOrchestrationServiceMock.Setup(expression: service =>
                service.QueueFlowDefinitionAsync(
                    flowDefinitionId,
                    asUserId,
                    args))
            .Throws(exception: dependencyException);

        Func<Task> action = async () => await coordinationService.QueueAsync(
            flowDefinitionId,
            asUserId,
            args);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task HandleFlowDefinitionDeleteAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        var flowDefinition = CreateRandomFlowDefinition();

        flowInstanceDataOrchestrationServiceMock.Setup(expression: service =>
                service.GetAll(true))
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await coordinationService.HandleFlowDefinitionDeleteAsync(flowDefinition);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005