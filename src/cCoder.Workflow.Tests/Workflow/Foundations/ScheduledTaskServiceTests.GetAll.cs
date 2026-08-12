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
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldGetAllScheduledTasks(bool ignoreFilters)
    {
        // Given
        IQueryable<ScheduledTask> expected =
            new[] { CreateScheduledTask() }
                .AsQueryable();

        if (ignoreFilters)
        {
            scheduledTaskBrokerMock
                .Setup(expression: broker => broker
                    .SelectAllScheduledTasksIgnoringQueryFilters())
                .Returns(value: expected);
        }
        else
        {
            scheduledTaskBrokerMock
                .Setup(expression: broker => broker.SelectAllScheduledTasks())
                .Returns(value: expected);
        }

        // When
        IQueryable<ScheduledTask> actual = scheduledTaskService.GetAll(
            ignoreFilters: ignoreFilters);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        scheduledTaskBrokerMock.VerifyAll();
    }
}