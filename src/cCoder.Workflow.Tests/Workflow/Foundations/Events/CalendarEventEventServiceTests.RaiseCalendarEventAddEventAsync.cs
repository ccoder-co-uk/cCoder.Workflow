// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class CalendarEventEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCalendarEventAddEventAsync()
    {
        // Given
        CalendarEvent entity = new();
        EventMessage<CalendarEvent> actualMessage = null;

        calendarEventEventBrokerMock
            .Setup(expression: x =>
                x.RaiseCalendarEventAddEventAsync(message: It.IsAny<EventMessage<CalendarEvent>>())
            )
            .Callback<EventMessage<CalendarEvent>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarEventAddEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        calendarEventEventBrokerMock.Verify(
expression: x => x.RaiseCalendarEventAddEventAsync(message: It.IsAny<EventMessage<CalendarEvent>>()),
times: Times.Once
        );

        calendarEventEventBrokerMock.Verify(
            expression: x => x.GetCurrentUserId(),
            times: Times.Once);

        calendarEventEventBrokerMock.VerifyNoOtherCalls();
    }

}