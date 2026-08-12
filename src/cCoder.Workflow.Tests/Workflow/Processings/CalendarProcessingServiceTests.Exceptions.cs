// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarServiceMock
            .Setup(expression: service => service.Get(calendarId: 1))
            .Throws(exception: exception);

        // When
        Action action = () => processingService.Get(calendarId: 1);

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
    public async Task ShouldMapDeleteAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        calendarServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarId: 1))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService.DeleteAsync(
            calendarId: 1);

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
    public async Task ShouldMapAddCalendarAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        calendarServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newCalendar: It.IsAny<Calendar>()))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService
            .AddCalendarAsync(newEntity: CreateCalendar());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}