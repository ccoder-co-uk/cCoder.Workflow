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

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock.Setup(expression: x => x.UpdateCalendarEventAsync(updatedEntity: entity))
            .ReturnsAsync(value: entity);

        calendarEventEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCalendarEventUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CalendarEvent result = await orchestrationService.UpdateCalendarEventAsync(updatedEntity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        calendarEventProcessingServiceMock.Verify(expression: x => x.UpdateCalendarEventAsync(updatedEntity: entity), times: Times.Once);
        calendarEventEventProcessingServiceMock.Verify(expression: x => x.RaiseCalendarEventUpdateEventAsync(entity: entity), times: Times.Once);
    }

}