// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectAllScheduledTasks())
            .Throws(exception: exception);

        // When
        Action action = () => scheduledTaskService.GetAll();

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
    public async Task ShouldMapMarkExecutedAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        ScheduledTask scheduledTask = CreateScheduledTask();

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectScheduledTaskForExecution(
                scheduledTaskId: scheduledTask.Id))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await scheduledTaskService
            .MarkExecutedAsync(
                scheduledTaskId: scheduledTask.Id,
                incrementNextExecution: true);

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
    public async Task ShouldMapDeleteAllByAppIdAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        int appId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .DeleteAllScheduledTasksByAppIdAsync(appId: appId))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await scheduledTaskService
            .DeleteAllByAppIdAsync(appId: appId);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}