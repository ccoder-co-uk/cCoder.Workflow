// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Models.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT008
public partial class CalendarEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateCalendarEvents()
    {
        // Given

        CalendarEvent added = new()
        {
            Id = 0,
            Name = "Added"
        };

        CalendarEvent updated = CreateCalendarEvent();

        calendarEventServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: added))
            .Returns(value: ValueTask.FromResult(result: added));

        calendarEventServiceMock
            .Setup(expression: service => service.UpdateCalendarEventAsync(
                updatedCalendarEvent: updated))
            .Returns(value: ValueTask.FromResult(result: updated));

        // When
        Result<CalendarEvent>[] results = (await processingService
            .AddOrUpdateCalendarEvent(items: new[] { added, updated }))
            .ToArray();

        // Then
        results
            .Should()
            .HaveCount(expected: 2);

        results
            .Should()
            .OnlyContain(predicate: result => result.Success);

        results[0].Message
            .Should()
            .Be(expected: "Added Successfully");

        results[1].Message
            .Should()
            .Be(expected: "Updated Successfully");

        calendarEventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateCalendarEventFailure()
    {
        // Given

        CalendarEvent item = new()
        {
            Id = 0,
            Name = "Failed"
        };

        calendarEventServiceMock
            .Setup(expression: service => service.AddCalendarEventAsync(
                newCalendarEvent: item))
            .Throws(exception: new Exception("failed"));

        // When
        Result<CalendarEvent> result = (await processingService
            .AddOrUpdateCalendarEvent(items: new[] { item }))
            .Single();

        // Then
        result.Success
            .Should()
            .BeFalse();

        result.Item
            .Should()
            .BeSameAs(expected: item);

        result.Message
            .Should()
            .Be(expected: "The Workflow service failed.");
    }
}
#pragma warning restore STXFORMAT008