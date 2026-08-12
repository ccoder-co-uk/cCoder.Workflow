// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class CalendarEntityEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCalendarAddEventAsync()
    {
        // Given
        Calendar entity = new();
        EventMessage<Calendar> actualMessage = null;

        calendarEntityEventBrokerMock
            .Setup(expression: x =>
                x.RaiseCalendarAddEventAsync(message: It.IsAny<EventMessage<Calendar>>())
            )
            .Callback<EventMessage<Calendar>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarAddEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        calendarEntityEventBrokerMock.Verify(
expression: x => x.RaiseCalendarAddEventAsync(message: It.IsAny<EventMessage<Calendar>>()),
times: Times.Once
        );

        calendarEntityEventBrokerMock.Verify(
            expression: x => x.GetCurrentUserId(),
            times: Times.Once);

        calendarEntityEventBrokerMock.VerifyNoOtherCalls();
    }

}