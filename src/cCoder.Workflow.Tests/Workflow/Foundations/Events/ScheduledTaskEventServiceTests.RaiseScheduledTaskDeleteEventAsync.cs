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
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseScheduledTaskDeleteEventAsync()
    {
        // Given
        ScheduledTask entity = new();
        EventMessage<ScheduledTask> actualMessage = null;

        scheduledTaskEventBrokerMock
            .Setup(expression: x =>
                x.RaiseScheduledTaskDeleteEventAsync(message: It.IsAny<EventMessage<ScheduledTask>>())
            )
            .Callback<EventMessage<ScheduledTask>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScheduledTaskDeleteEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        scheduledTaskEventBrokerMock.Verify(
expression: x => x.RaiseScheduledTaskDeleteEventAsync(message: It.IsAny<EventMessage<ScheduledTask>>()),
times: Times.Once
        );

        scheduledTaskEventBrokerMock.Verify(
            expression: x => x.GetCurrentUserId(),
            times: Times.Once);

        scheduledTaskEventBrokerMock.VerifyNoOtherCalls();
    }

}