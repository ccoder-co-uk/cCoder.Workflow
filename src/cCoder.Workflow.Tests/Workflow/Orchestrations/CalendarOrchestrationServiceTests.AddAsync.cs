// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT008, STXFORMAT009
public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Calendar entity = CreateRandomCalendar();

        calendarProcessingServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newEntity: entity))
            .ReturnsAsync(value: entity);

        eventServiceMock
            .Setup(expression: service => service.RaiseCalendarAddEventAsync(
                entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Calendar result = await orchestrationService
            .AddCalendarAsync(newEntity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        calendarProcessingServiceMock.Verify(
            expression: service => service.AddCalendarAsync(
                newEntity: entity),
            times: Times.Once);

        eventServiceMock.Verify(
            expression: service => service.RaiseCalendarAddEventAsync(
                entity: entity),
            times: Times.Once);
    }

}
#pragma warning restore STXFORMAT008, STXFORMAT009