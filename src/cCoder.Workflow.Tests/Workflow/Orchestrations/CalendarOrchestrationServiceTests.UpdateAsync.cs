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

public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Calendar entity = CreateRandomCalendar();

        calendarProcessingServiceMock.Setup(expression: x => x.UpdateCalendarAsync(updatedEntity: entity))
            .ReturnsAsync(value: entity);

        eventServiceMock
            .Setup(expression: x => x.RaiseCalendarUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Calendar result = await orchestrationService.UpdateCalendarAsync(updatedEntity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        calendarProcessingServiceMock.Verify(expression: x => x.UpdateCalendarAsync(updatedEntity: entity), times: Times.Once);
        eventServiceMock.Verify(expression: x => x.RaiseCalendarUpdateEventAsync(entity: entity), times: Times.Once);
    }

}