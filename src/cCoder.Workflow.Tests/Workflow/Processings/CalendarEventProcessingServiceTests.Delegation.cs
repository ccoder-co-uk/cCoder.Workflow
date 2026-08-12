// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateCalendarEventOperationsAsync()
    {
        // Given
        CalendarEvent task = CreateCalendarEvent();
        IQueryable<CalendarEvent> tasks = new[] { task }
            .AsQueryable();

        calendarEventServiceMock
            .Setup(expression: service => service.Get(
                calendarEventId: task.Id))
            .Returns(value: task);

        calendarEventServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: tasks);

        calendarEventServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: task))
            .Returns(value: ValueTask.FromResult(result: task));

        calendarEventServiceMock
            .Setup(expression: service => service.UpdateCalendarEventAsync(
                updatedCalendarEvent: task))
            .Returns(value: ValueTask.FromResult(result: task));

        calendarEventServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarEventId: task.Id))
            .Returns(value: ValueTask.CompletedTask);

        calendarEventServiceMock
            .Setup(expression: service => service.DeleteAllByAppIdAsync(
                appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        calendarEventServiceMock
            .Setup(expression: service => service
                .DeleteAllForAppCalendarEventAsync(
                    deletedItems: It.Is<IEnumerable<CalendarEvent>>(
                        match: items => items.Single() == task)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CalendarEvent actualGet = processingService.Get(calendarEventId: task.Id);

        IQueryable<CalendarEvent> actualAll = processingService.GetAll(
            ignoreFilters: true);

        CalendarEvent actualAdded = await processingService.AddCalendarEventAsync(
            newEntity: task);

        CalendarEvent actualUpdated = await processingService.UpdateCalendarEventAsync(
            updatedEntity: task);

        await processingService.DeleteAsync(calendarEventId: task.Id);
        await processingService.DeleteAllForAppCalendarEventAsync(
            deletedItems: new[] { task });

        await processingService.DeleteAllByAppIdAsync(appId: 7);

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

        calendarEventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarEventsAsync()
    {
        // Given
        CalendarEvent first = CreateCalendarEvent();
        CalendarEvent second = CreateCalendarEvent();

        calendarEventServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarEventId: first.Id))
            .Returns(value: ValueTask.CompletedTask);

        calendarEventServiceMock
            .Setup(expression: service => service.DeleteAsync(
                calendarEventId: second.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await processingService.DeleteAllCalendarEventAsync(
            deletedItems: new[] { first, second });

        // Then
        calendarEventServiceMock.VerifyAll();
    }
}