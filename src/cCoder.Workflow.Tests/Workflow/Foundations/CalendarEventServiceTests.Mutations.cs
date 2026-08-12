// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarEventServiceTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldSaveCalendarAsync(bool isUpdate)
    {
        // Given
        CalendarEvent input = CreateCalendarEvent();
        CalendarEvent stored = CreateCalendarEvent();
        const int appId = 7;
        string privilege = isUpdate
            ? "CalendarEvent_update"
            : "CalendarEvent_create";

        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAppId(entity: input))
            .Returns(value: appId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: appId,
                privilege: privilege));

        if (isUpdate)
        {
            calendarEventBrokerMock
                .Setup(expression: broker => broker.UpdateCalendarEventAsync(
                    updatedEntity: It.Is<CalendarEvent>(match: item =>
                        item.Id == input.Id)))
                .Returns(value: ValueTask.FromResult(result: stored));
        }
        else
        {
            calendarEventBrokerMock
                .Setup(expression: broker => broker.InsertCalendarEventAsync(
                    newEntity: It.Is<CalendarEvent>(match: item =>
                        item.Name == input.Name)))
                .Returns(value: ValueTask.FromResult(result: stored));
        }

        // When
        CalendarEvent actual = isUpdate
            ? await calendarEventService.UpdateCalendarEventAsync(
                updatedCalendarEvent: input)
            : await calendarEventService.AddCalendarEventAsync(
                newCalendarEvent: input);

        // Then
        actual
            .Should()
            .BeSameAs(expected: input);

        actual.Id
            .Should()
            .Be(expected: stored.Id);

        actual.Name
            .Should()
            .Be(expected: stored.Name);

        calendarEventBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteCalendarEventAsync()
    {
        // Given
        CalendarEvent calendarEvent = CreateCalendarEvent();
        const int appId = 7;

        calendarEventBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarEventsIgnoringQueryFilters())
            .Returns(value: new[] { calendarEvent }
                .AsQueryable());

        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAppId(
                entity: calendarEvent))
            .Returns(value: appId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: appId,
                privilege: "CalendarEvent_delete"));

        calendarEventBrokerMock
            .Setup(expression: broker => broker.DeleteCalendarEventAsync(
                deletedEntity: It.Is<CalendarEvent>(match: deleted =>
                    deleted.Id == calendarEvent.Id)))
            .Returns(value: ValueTask.FromResult(result: 1));

        // When
        await calendarEventService.DeleteAsync(
            calendarEventId: calendarEvent.Id);

        // Then
        calendarEventBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreMissingCalendarWhenDeleteAsync()
    {
        // Given
        calendarEventBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarEventsIgnoringQueryFilters())
            .Returns(value: Array.Empty<CalendarEvent>()
                .AsQueryable());

        // When
        await calendarEventService.DeleteAsync(calendarEventId: 1);

        // Then
        calendarEventBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarEventsAsync()
    {
        // Given
        CalendarEvent calendarEvent = CreateCalendarEvent();

        calendarEventBrokerMock
            .Setup(expression: broker => broker.DeleteAllCalendarEventsAsync(
                deletedItems: It.Is<IEnumerable<CalendarEvent>>(match: items =>
                    items.Single().Id == calendarEvent.Id)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await calendarEventService.DeleteAllForAppCalendarEventAsync(
            deletedItems: new[] { calendarEvent });

        // Then
        calendarEventBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreEmptyDeleteAllCalendarEventsAsync()
    {
        // Given

        // When
        await calendarEventService.DeleteAllForAppCalendarEventAsync(
            deletedItems: Array.Empty<CalendarEvent>());

        // Then
        calendarEventBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarEventsByAppIdAsync()
    {
        // Given
        const int appId = 7;

        calendarEventBrokerMock
            .Setup(expression: broker => broker
                .DeleteAllCalendarEventsByAppIdAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await calendarEventService.DeleteAllByAppIdAsync(appId: appId);

        // Then
        calendarEventBrokerMock.VerifyAll();
    }
}