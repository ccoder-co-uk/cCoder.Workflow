// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class ScheduledTaskProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        scheduledTaskServiceMock
            .Setup(expression: service => service.Get(scheduledTaskId: 1))
            .Throws(exception: exception);

        // When
        Action action = () => processingService.Get(scheduledTaskId: 1);

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        scheduledTaskServiceMock
            .Setup(expression: service => service.DeleteAsync(
                scheduledTaskId: 1))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService.DeleteAsync(
            scheduledTaskId: 1);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapExecuteScheduledTaskAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        scheduledTaskServiceMock
            .Setup(expression: service => service.MarkExecutedAsync(
                scheduledTaskId: 1,
                incrementNextExecution: true))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService
            .ExecuteScheduledTaskAsync(scheduledTaskId: 1);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}