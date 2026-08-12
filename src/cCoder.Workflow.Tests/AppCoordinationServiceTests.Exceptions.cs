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

namespace cCoder.Workflow.Tests;

public partial class AppCoordinationServiceTests
{
    public static TheoryData<Exception, Type> DeleteDependencyExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), typeof(WorkflowValidationException) },
        { new WorkflowDependencyException(innerException: new Exception()), typeof(WorkflowDependencyException) },
        { new ValidationException(), typeof(WorkflowValidationException) },
        { new InvalidOperationException(), typeof(WorkflowDependencyException) },
        { new SecurityException(), typeof(SecurityException) },
        { new Exception(), typeof(WorkflowServiceException) }
    };

    [Theory]
    [MemberData(nameof(DeleteDependencyExceptions))]
    public async Task DeleteAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        scheduledTaskOrchestrationServiceMock.Setup(expression: dependency =>
                dependency.DeleteByAppIdAsync(appId: 5))
            .Throws(exception: dependencyException);

        Func<Task> action = async () => await service.DeleteAsync(appId: 5);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005