// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateAddOrUpdateCalendarsAsync()
    {
        // Given
        Calendar item = CreateRandomCalendar();
        Calendar[] items = [item];
        Result<Calendar>[] expected = [new() { Success = true, Item = item }];

        calendarProcessingServiceMock
            .Setup(expression: service => service.AddOrUpdateCalendar(
                items: items))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<Calendar>>>(
                result: expected));

        // When
        IEnumerable<Result<Calendar>> actual = await orchestrationService
            .AddOrUpdateCalendar(items: items);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        calendarProcessingServiceMock.VerifyAll();
    }
}