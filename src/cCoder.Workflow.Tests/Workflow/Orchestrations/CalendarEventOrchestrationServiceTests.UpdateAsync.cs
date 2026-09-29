// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock.Setup(expression: x => x.UpdateCalendarEventAsync(updatedCalendarEvent: entity))
            .ReturnsAsync(value: entity);

        calendarEventEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCalendarEventUpdateEventAsync(calendarEvent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CalendarEvent result = await orchestrationService.UpdateCalendarEventAsync(updatedCalendarEvent: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);


        calendarEventProcessingServiceMock.Verify(expression: x => x.UpdateCalendarEventAsync(updatedCalendarEvent: entity), times: Times.Once);
        calendarEventEventProcessingServiceMock.Verify(expression: x => x.RaiseCalendarEventUpdateEventAsync(calendarEvent: entity), times: Times.Once);
    }

}