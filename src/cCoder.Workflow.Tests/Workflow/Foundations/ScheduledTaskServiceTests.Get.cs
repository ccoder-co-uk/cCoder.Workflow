// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public void ShouldGetScheduledTask()
    {
        // Given
        ScheduledTask expected = CreateScheduledTask();

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectAllScheduledTasks())
            .Returns(value: new[] { expected }
                .AsQueryable());

        // When
        ScheduledTask actual = scheduledTaskService.Get(
            scheduledTaskId: expected.Id);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldReturnNullWhenScheduledTaskDoesNotExist()
    {
        // Given
        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectAllScheduledTasks())
            .Returns(value: Array.Empty<ScheduledTask>()
                .AsQueryable());

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectAllScheduledTasksIgnoringQueryFilters())
            .Returns(value: Array.Empty<ScheduledTask>()
                .AsQueryable());

        // When
        ScheduledTask actual = scheduledTaskService.Get(scheduledTaskId: 1);

        // Then
        actual
            .Should()
            .BeNull();

        scheduledTaskBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldRejectFilteredScheduledTask()
    {
        // Given
        ScheduledTask restricted = CreateScheduledTask();

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectAllScheduledTasks())
            .Returns(value: Array.Empty<ScheduledTask>()
                .AsQueryable());

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectAllScheduledTasksIgnoringQueryFilters())
            .Returns(value: new[] { restricted }
                .AsQueryable());

        // When
        Action action = () => scheduledTaskService.Get(
            scheduledTaskId: restricted.Id);

        // Then
        action
            .Should()
            .Throw<SecurityException>();

        scheduledTaskBrokerMock.VerifyAll();
    }
}