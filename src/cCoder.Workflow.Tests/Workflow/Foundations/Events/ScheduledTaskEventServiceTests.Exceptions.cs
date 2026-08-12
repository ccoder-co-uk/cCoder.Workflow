// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class ScheduledTaskEventServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .ScheduledTaskServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseScheduledTaskAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        ScheduledTask scheduledTask = new() { Id = 1 };

        scheduledTaskEventBrokerMock
            .Setup(expression: broker => broker.RaiseScheduledTaskAddEventAsync(
                message: It.Is<EventMessage<ScheduledTask>>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseScheduledTaskAddEventAsync(entity: scheduledTask);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}