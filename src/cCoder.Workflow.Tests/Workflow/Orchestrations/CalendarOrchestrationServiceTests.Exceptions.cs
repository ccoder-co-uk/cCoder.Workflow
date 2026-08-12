// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        calendarProcessingServiceMock
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
    public async Task ShouldMapAddCalendarAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Calendar item = CreateRandomCalendar();

        calendarProcessingServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newEntity: item))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .AddCalendarAsync(newEntity: item);

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
    public async Task ShouldMapDeleteAllCalendarAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Calendar[] items = [CreateRandomCalendar()];

        calendarProcessingServiceMock
            .Setup(expression: service => service.DeleteAllCalendarAsync(
                deletedItems: items))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .DeleteAllCalendarAsync(deletedItems: items);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}