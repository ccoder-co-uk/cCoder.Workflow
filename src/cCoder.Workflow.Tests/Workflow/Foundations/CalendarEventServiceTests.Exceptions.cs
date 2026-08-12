// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarEventServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendarEvents())
            .Throws(exception: exception);

        // When
        Action action = () => calendarEventService.GetAll();

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
    public async Task ShouldMapAddCalendarEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        CalendarEvent calendarEvent = CreateCalendarEvent();

        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAppId(
                entity: calendarEvent))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await calendarEventService
            .AddCalendarEventAsync(newCalendarEvent: calendarEvent);

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
        calendarEventBrokerMock
            .Setup(expression: broker => broker.DeleteAllCalendarEventsByAppIdAsync(
                appId: 1))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await calendarEventService
            .DeleteAllByAppIdAsync(appId: 1);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}