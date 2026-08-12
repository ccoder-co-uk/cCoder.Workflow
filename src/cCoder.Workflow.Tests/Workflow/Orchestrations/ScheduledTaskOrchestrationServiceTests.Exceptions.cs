// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005
public partial class ScheduledTaskOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        Foundations.FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        processingServiceMock.Setup(expression: found => found.Get(1))
            .Throws(exception);

        Action action = () => service.Get(scheduledTaskId: 1);

        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteFailureAsync(Exception exception, Type expectedType)
    {
        processingServiceMock.Setup(expression: found => found.GetAll(true))
            .Throws(exception);

        Func<Task> action = async () => await service.DeleteAsync(scheduledTaskId: 1);

        Exception thrown = (await action.Should().ThrowAsync<Exception>()).Which;
        thrown.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddFailureAsync(Exception exception, Type expectedType)
    {
        ScheduledTask task = CreateScheduledTask();
        processingServiceMock.Setup(expression: found => found.AddScheduledTaskAsync(task))
            .Throws(exception);

        Func<Task> action = async () => await service.AddScheduledTaskAsync(task);

        Exception thrown = (await action.Should().ThrowAsync<Exception>()).Which;
        thrown.Should().BeOfType(expectedType);
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005