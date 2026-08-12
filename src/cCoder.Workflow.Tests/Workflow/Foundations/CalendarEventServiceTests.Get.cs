// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarEventServiceTests
{
    [Fact]
    public void ShouldGetCalendar()
    {
        // Given
        CalendarEvent expected = CreateCalendarEvent();

        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendarEvents())
            .Returns(value: new[] { expected }
                .AsQueryable());

        // When
        CalendarEvent actual = calendarEventService.Get(
            calendarEventId: expected.Id);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);
    }

    [Fact]
    public void ShouldReturnNullForMissingCalendar()
    {
        // Given
        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendarEvents())
            .Returns(value: Array.Empty<CalendarEvent>()
                .AsQueryable());

        calendarEventBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarEventsIgnoringQueryFilters())
            .Returns(value: Array.Empty<CalendarEvent>()
                .AsQueryable());

        // When
        CalendarEvent actual = calendarEventService.Get(calendarEventId: 1);

        // Then
        actual
            .Should()
            .BeNull();
    }

    [Fact]
    public void ShouldRejectFilteredCalendar()
    {
        // Given
        CalendarEvent restricted = CreateCalendarEvent();

        calendarEventBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendarEvents())
            .Returns(value: Array.Empty<CalendarEvent>()
                .AsQueryable());

        calendarEventBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarEventsIgnoringQueryFilters())
            .Returns(value: new[] { restricted }
                .AsQueryable());

        // When
        Action action = () => calendarEventService.Get(
            calendarEventId: restricted.Id);

        // Then
        action
            .Should()
            .Throw<SecurityException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldGetAllCalendars(bool ignoreFilters)
    {
        // Given
        IQueryable<CalendarEvent> expected = new[] { CreateCalendarEvent() }
            .AsQueryable();

        if (ignoreFilters)
        {
            calendarEventBrokerMock
                .Setup(expression: broker => broker
                    .SelectAllCalendarEventsIgnoringQueryFilters())
                .Returns(value: expected);
        }
        else
        {
            calendarEventBrokerMock
                .Setup(expression: broker => broker.SelectAllCalendarEvents())
                .Returns(value: expected);
        }

        // When
        IQueryable<CalendarEvent> actual = calendarEventService.GetAll(
            ignoreFilters: ignoreFilters);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);
    }
}