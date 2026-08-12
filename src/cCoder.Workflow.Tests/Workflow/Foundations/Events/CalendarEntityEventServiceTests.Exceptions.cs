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
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .CalendarServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseCalendarAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Calendar calendar = new() { Id = 1 };

        calendarEntityEventBrokerMock
            .Setup(expression: broker => broker.RaiseCalendarAddEventAsync(
                message: It.Is<EventMessage<Calendar>>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseCalendarAddEventAsync(entity: calendar);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}