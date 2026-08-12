// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateCalendarOperationsAsync()
    {
        // Given
        Calendar task = CreateCalendar();
        IQueryable<Calendar> tasks = new[] { task }
            .AsQueryable();

        calendarServiceMock
            .Setup(expression: service => service.Get(
                calendarId: task.Id))
            .Returns(value: task);

        calendarServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: tasks);

        calendarServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newCalendar: task))
            .Returns(value: ValueTask.FromResult(result: task));

        calendarServiceMock
            .Setup(expression: service => service.UpdateCalendarAsync(
                updatedCalendar: task))
            .Returns(value: ValueTask.FromResult(result: task));

        calendarServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarId: task.Id))
            .Returns(value: ValueTask.CompletedTask);

        calendarServiceMock
            .Setup(expression: service => service.DeleteAllByAppIdAsync(
                appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Calendar actualGet = processingService.Get(calendarId: task.Id);

        IQueryable<Calendar> actualAll = processingService.GetAll(
            ignoreFilters: true);

        Calendar actualAdded = await processingService.AddCalendarAsync(
            newEntity: task);

        Calendar actualUpdated = await processingService.UpdateCalendarAsync(
            updatedEntity: task);

        await processingService.DeleteAsync(calendarId: task.Id);
        await processingService.DeleteByAppIdAsync(appId: 7);

        // Then
        actualGet
            .Should()
            .BeSameAs(expected: task);

        actualAll
            .Should()
            .BeSameAs(expected: tasks);

        actualAdded
            .Should()
            .BeSameAs(expected: task);

        actualUpdated
            .Should()
            .BeSameAs(expected: task);

        calendarServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarsAsync()
    {
        // Given
        Calendar first = CreateCalendar();
        Calendar second = CreateCalendar();

        calendarServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarId: first.Id))
            .Returns(value: ValueTask.CompletedTask);

        calendarServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarId: second.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await processingService.DeleteAllCalendarAsync(
            deletedItems: new[] { first, second });

        // Then
        calendarServiceMock.VerifyAll();
    }
}