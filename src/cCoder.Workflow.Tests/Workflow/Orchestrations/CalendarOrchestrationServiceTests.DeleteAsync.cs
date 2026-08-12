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

#pragma warning disable STXFORMAT009
public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        const int id = 1;
        Calendar entity = CreateRandomCalendar();
        CalendarEvent calendarEvent = new() { CalendarId = entity.Id };

        calendarProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        calendarProcessingServiceMock.Setup(expression: x => x.DeleteAsync(calendarId: id))
            .Returns(value: ValueTask.CompletedTask);

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Returns(value: new[] { calendarEvent }.AsQueryable());

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.DeleteAllCalendarEventAsync(
                deletedItems: It.Is<IEnumerable<CalendarEvent>>(
                    match: items => items.Single() == calendarEvent)))
            .Returns(value: ValueTask.CompletedTask);

        eventServiceMock
            .Setup(expression: service => service.RaiseCalendarDeleteEventAsync(
                entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(calendarId: id);

        // Then
        calendarProcessingServiceMock.Verify(
            expression: service => service.GetAll(ignoreFilters: true),
            times: Times.Once);
        calendarProcessingServiceMock.Verify(expression: x => x.DeleteAsync(calendarId: id), times: Times.Once);
        calendarEventProcessingServiceMock.VerifyAll();
        eventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreMissingCalendarWhenDeleteAsync()
    {
        // Given
        calendarProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Calendar>().AsQueryable());

        // When
        await orchestrationService.DeleteAsync(calendarId: 1);

        // Then
        calendarProcessingServiceMock.VerifyAll();
        calendarEventProcessingServiceMock.VerifyNoOtherCalls();
        eventServiceMock.VerifyNoOtherCalls();
    }

}
#pragma warning restore STXFORMAT009