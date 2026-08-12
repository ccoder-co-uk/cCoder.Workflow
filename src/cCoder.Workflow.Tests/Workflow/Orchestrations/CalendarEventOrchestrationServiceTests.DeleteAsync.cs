// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        const int id = 1;
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        calendarEventProcessingServiceMock.Setup(expression: x => x.DeleteAsync(calendarEventId: id))
            .Returns(value: ValueTask.CompletedTask);

        calendarEventEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCalendarEventDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(calendarEventId: id);

        // Then
        calendarEventProcessingServiceMock.Verify(
            expression: service => service.GetAll(ignoreFilters: true),
            times: Times.Once);
        calendarEventProcessingServiceMock.Verify(expression: x => x.DeleteAsync(calendarEventId: id), times: Times.Once);
        calendarEventEventProcessingServiceMock.Verify(expression: x => x.RaiseCalendarEventDeleteEventAsync(entity: entity), times: Times.Once);
    }

    [Fact]
    public async Task ShouldIgnoreMissingCalendarEventWhenDeleteAsync()
    {
        // Given
        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<CalendarEvent>().AsQueryable());

        // When
        await orchestrationService.DeleteAsync(calendarEventId: 1);

        // Then
        calendarEventProcessingServiceMock.VerifyAll();
        calendarEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}