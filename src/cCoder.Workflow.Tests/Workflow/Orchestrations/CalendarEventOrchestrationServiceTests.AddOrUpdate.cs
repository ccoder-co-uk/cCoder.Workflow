// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateAddOrUpdateCalendarEventsAsync()
    {
        // Given
        CalendarEvent item = CreateRandomCalendarEvent();
        CalendarEvent[] items = [item];
        Result<CalendarEvent>[] expected = [new() { Success = true, Item = item }];

        calendarEventProcessingServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendarEvent(
                items: items))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<CalendarEvent>>>(
                result: expected));

        // When
        IEnumerable<Result<CalendarEvent>> actual = await orchestrationService
            .AddOrUpdateCalendarEvent(items: items);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        calendarEventProcessingServiceMock.VerifyAll();
    }
}