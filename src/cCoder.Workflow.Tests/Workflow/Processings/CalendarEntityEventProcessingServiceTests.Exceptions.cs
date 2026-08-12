// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEntityEventProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseCalendarAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Calendar entity = CreateRandomCalendar();

        calendarEntityEventServiceMock
            .Setup(expression: dependency => dependency
                .RaiseCalendarAddEventAsync(entity: entity))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseCalendarAddEventAsync(entity: entity);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}