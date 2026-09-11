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
public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: entity))
            .ReturnsAsync(value: entity);

        calendarEventEventProcessingServiceMock
            .Setup(expression: service => service.RaiseCalendarEventAddEventAsync(
                calendarEvent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CalendarEvent result = await orchestrationService
            .AddCalendarEventAsync(newCalendarEvent: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);


        calendarEventProcessingServiceMock.Verify(
            expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: entity),
            times: Times.Once);

        calendarEventEventProcessingServiceMock.Verify(
            expression: service => service.RaiseCalendarEventAddEventAsync(
                calendarEvent: entity),
            times: Times.Once);
    }

}
#pragma warning restore STXFORMAT008, STXFORMAT009