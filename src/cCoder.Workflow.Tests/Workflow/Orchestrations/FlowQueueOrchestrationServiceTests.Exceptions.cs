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

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowQueueOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> QueueDependencyExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), typeof(WorkflowValidationException) },
        { new WorkflowDependencyException(innerException: new Exception()), typeof(WorkflowDependencyException) },
        { new ValidationException(), typeof(WorkflowValidationException) },
        { new InvalidOperationException(), typeof(WorkflowDependencyException) },
        { new SecurityException(), typeof(SecurityException) },
        { new Exception(), typeof(WorkflowServiceException) }
    };

    [Theory]
    [MemberData(nameof(QueueDependencyExceptions))]
    public async Task QueueFlowDefinitionAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        Guid flowDefinitionId = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString();
        string args = "{}";

        flowDefinitionProcessingServiceMock.Setup(expression: dependency =>
                dependency.GetAll(true))
            .Throws(exception: dependencyException);

        Func<Task> action = async () => await orchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId,
            asUserId,
            args);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005