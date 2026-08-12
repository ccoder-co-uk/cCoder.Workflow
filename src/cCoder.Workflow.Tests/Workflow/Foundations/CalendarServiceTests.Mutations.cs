// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarServiceTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldSaveCalendarAsync(bool isUpdate)
    {
        // Given
        Calendar input = CreateCalendar();
        Calendar stored = CreateCalendar();
        string privilege = isUpdate
            ? "Calendar_update"
            : "Calendar_create";

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: input.AppId,
                privilege: privilege));

        if (isUpdate)
        {
            calendarBrokerMock
                .Setup(expression: broker => broker.UpdateCalendarAsync(
                    updatedEntity: It.Is<Calendar>(match: item =>
                        item.Id == input.Id)))
                .Returns(value: ValueTask.FromResult(result: stored));
        }
        else
        {
            calendarBrokerMock
                .Setup(expression: broker => broker.InsertCalendarAsync(
                    newEntity: It.Is<Calendar>(match: item =>
                        item.Name == input.Name)))
                .Returns(value: ValueTask.FromResult(result: stored));
        }

        // When
        Calendar actual = isUpdate
            ? await calendarService.UpdateCalendarAsync(
                updatedCalendar: input)
            : await calendarService.AddCalendarAsync(
                newCalendar: input);

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

        calendarBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteCalendarAsync()
    {
        // Given
        Calendar calendar = CreateCalendar();

        calendarBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarsIgnoringQueryFilters())
            .Returns(value: new[] { calendar }
                .AsQueryable());

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: calendar.AppId,
                privilege: "Calendar_delete"));

        calendarBrokerMock
            .Setup(expression: broker => broker.DeleteCalendarAsync(
                deletedEntity: It.Is<Calendar>(match: deleted =>
                    deleted.Id == calendar.Id)))
            .Returns(value: ValueTask.FromResult(result: 1));

        // When
        await calendarService.DeleteAsync(calendarId: calendar.Id);

        // Then
        calendarBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreMissingCalendarWhenDeleteAsync()
    {
        // Given
        calendarBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarsIgnoringQueryFilters())
            .Returns(value: Array.Empty<Calendar>()
                .AsQueryable());

        // When
        await calendarService.DeleteAsync(calendarId: 1);

        // Then
        calendarBrokerMock.VerifyAll();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarsAsync()
    {
        // Given
        Calendar calendar = CreateCalendar();

        calendarBrokerMock
            .Setup(expression: broker => broker.DeleteAllCalendarsAsync(
                deletedItems: It.Is<IEnumerable<Calendar>>(match: items =>
                    items.Single().Id == calendar.Id)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await calendarService.DeleteAllForAppCalendarAsync(
            deletedItems: new[] { calendar });

        // Then
        calendarBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreEmptyDeleteAllCalendarsAsync()
    {
        // Given

        // When
        await calendarService.DeleteAllForAppCalendarAsync(
            deletedItems: Array.Empty<Calendar>());

        // Then
        calendarBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteAllCalendarsByAppIdAsync()
    {
        // Given
        const int appId = 7;

        calendarBrokerMock
            .Setup(expression: broker => broker
                .DeleteAllCalendarsByAppIdAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await calendarService.DeleteAllByAppIdAsync(appId: appId);

        // Then
        calendarBrokerMock.VerifyAll();
    }
}