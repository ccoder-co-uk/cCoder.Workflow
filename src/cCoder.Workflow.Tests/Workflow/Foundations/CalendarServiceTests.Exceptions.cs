// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendars())
            .Throws(exception: exception);

        // When
        Action action = () => calendarService.GetAll();

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
    public async Task ShouldMapAddCalendarAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Calendar calendar = CreateCalendar();

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: calendar.AppId,
                privilege: "Calendar_create"))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await calendarService
            .AddCalendarAsync(newCalendar: calendar);

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
        calendarBrokerMock
            .Setup(expression: broker => broker.DeleteAllCalendarsByAppIdAsync(
                appId: 1))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await calendarService
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