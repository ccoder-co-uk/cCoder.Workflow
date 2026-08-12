// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class CalendarServiceTests
{
    [Fact]
    public void ShouldGetCalendar()
    {
        // Given
        Calendar expected = CreateCalendar();

        calendarBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendars())
            .Returns(value: new[] { expected }
                .AsQueryable());

        // When
        Calendar actual = calendarService.Get(calendarId: expected.Id);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);
    }

    [Fact]
    public void ShouldReturnNullForMissingCalendar()
    {
        // Given
        calendarBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendars())
            .Returns(value: Array.Empty<Calendar>()
                .AsQueryable());

        calendarBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarsIgnoringQueryFilters())
            .Returns(value: Array.Empty<Calendar>()
                .AsQueryable());

        // When
        Calendar actual = calendarService.Get(calendarId: 1);

        // Then
        actual
            .Should()
            .BeNull();
    }

    [Fact]
    public void ShouldRejectFilteredCalendar()
    {
        // Given
        Calendar restricted = CreateCalendar();

        calendarBrokerMock
            .Setup(expression: broker => broker.SelectAllCalendars())
            .Returns(value: Array.Empty<Calendar>()
                .AsQueryable());

        calendarBrokerMock
            .Setup(expression: broker => broker
                .SelectAllCalendarsIgnoringQueryFilters())
            .Returns(value: new[] { restricted }
                .AsQueryable());

        // When
        Action action = () => calendarService.Get(
            calendarId: restricted.Id);

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
        IQueryable<Calendar> expected = new[] { CreateCalendar() }
            .AsQueryable();

        if (ignoreFilters)
        {
            calendarBrokerMock
                .Setup(expression: broker => broker
                    .SelectAllCalendarsIgnoringQueryFilters())
                .Returns(value: expected);
        }
        else
        {
            calendarBrokerMock
                .Setup(expression: broker => broker.SelectAllCalendars())
                .Returns(value: expected);
        }

        // When
        IQueryable<Calendar> actual = calendarService.GetAll(
            ignoreFilters: ignoreFilters);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);
    }
}