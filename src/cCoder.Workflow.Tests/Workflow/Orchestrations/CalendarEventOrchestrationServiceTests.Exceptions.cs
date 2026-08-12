// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        // When
        Action action = () => orchestrationService.GetAll();

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
    public async Task ShouldMapAddCalendarEventAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        CalendarEvent item = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newEntity: item))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .AddCalendarEventAsync(newEntity: item);

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
    public async Task ShouldMapDeleteAllCalendarEventAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        CalendarEvent[] items = [CreateRandomCalendarEvent()];

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.DeleteAllCalendarEventAsync(
                deletedItems: items))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .DeleteAllCalendarEventAsync(deletedItems: items);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}