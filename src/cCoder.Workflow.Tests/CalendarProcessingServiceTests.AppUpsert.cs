// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class CalendarProcessingServiceAppUpsertTests
{
    [Fact]
    public async Task ShouldAddCalendarWhenSuppliedIdDoesNotExist()
    {
        // Given
        Calendar calendar = new() { Id = 42, AppId = 7, Name = "Imported" };
        Mock<ICalendarService> calendarServiceMock = new(behavior: MockBehavior.Strict);

        calendarServiceMock.Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Calendar>()
                .AsQueryable());

        calendarServiceMock.Setup(expression: service => service.AddCalendarAsync(newCalendar: calendar))
            .Returns(value: ValueTask.FromResult(result: calendar));

        CalendarProcessingService service = new(service: calendarServiceMock.Object);

        // When
        _ = await service.AddOrUpdateCalendar(items: [calendar]);

        // Then
        calendarServiceMock.Verify(expression: service => service.AddCalendarAsync(newCalendar: calendar), times: Times.Once);
        calendarServiceMock.Verify(expression: service => service.UpdateCalendarAsync(updatedCalendar: It.IsAny<Calendar>()), times: Times.Never);
    }
}