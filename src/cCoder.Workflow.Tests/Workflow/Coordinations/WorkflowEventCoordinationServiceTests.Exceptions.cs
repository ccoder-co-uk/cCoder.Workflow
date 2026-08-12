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

public partial class WorkflowEventCoordinationServiceTests
{
    public static TheoryData<Exception, Type> RaiseEventDependencyExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), typeof(WorkflowValidationException) },
        { new WorkflowDependencyException(innerException: new Exception()), typeof(WorkflowDependencyException) },
        { new ValidationException(), typeof(WorkflowValidationException) },
        { new InvalidOperationException(), typeof(WorkflowDependencyException) },
        { new SecurityException(), typeof(SecurityException) },
        { new Exception(), typeof(WorkflowServiceException) }
    };

    [Theory]
    [MemberData(nameof(RaiseEventDependencyExceptions))]
    public async Task RaiseEventsShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        object payload = new();

        workflowEventOrchestrationServiceMock.Setup(expression: dependency =>
                dependency.PrepareWorkflowEventDispatch(
                    payload,
                    "event",
                    null))
            .Throws(exception: dependencyException);

        Func<Task> action = async () => await coordinationService.RaiseEvents(
            payload,
            "event");

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005