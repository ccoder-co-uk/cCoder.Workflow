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
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .CalendarEventServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseCalendarEventAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        CalendarEvent calendarEvent = new() { Id = 1 };

        calendarEventEventBrokerMock
            .Setup(expression: broker => broker.RaiseCalendarEventAddEventAsync(
                message: It.Is<EventMessage<CalendarEvent>>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseCalendarEventAddEventAsync(entity: calendarEvent);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}