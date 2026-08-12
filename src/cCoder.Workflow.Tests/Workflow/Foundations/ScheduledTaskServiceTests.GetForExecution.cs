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
    [Fact]
    public void ShouldGetScheduledTaskForExecution()
    {
        // Given
        ScheduledTask expected = CreateScheduledTask();

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectScheduledTaskForExecution(
                scheduledTaskId: expected.Id))
            .Returns(value: expected);

        // When
        ScheduledTask actual = scheduledTaskService.GetForExecution(
            scheduledTaskId: expected.Id);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        scheduledTaskBrokerMock.VerifyAll();
    }
}