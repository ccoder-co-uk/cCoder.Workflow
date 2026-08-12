// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEventProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarEventServiceMock
            .Setup(expression: service => service.Get(calendarEventId: 1))
            .Throws(exception: exception);

        // When
        Action action = () => processingService.Get(calendarEventId: 1);

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
        calendarEventServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarEventId: 1))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService.DeleteAsync(
            calendarEventId: 1);

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
    public async Task ShouldMapAddCalendarEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        calendarEventServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: It.IsAny<CalendarEvent>()))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await processingService
            .AddCalendarEventAsync(newEntity: CreateCalendarEvent());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}