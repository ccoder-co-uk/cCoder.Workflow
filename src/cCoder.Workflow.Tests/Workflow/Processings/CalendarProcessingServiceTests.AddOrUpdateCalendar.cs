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
public partial class CalendarProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateCalendars()
    {
        // Given

        Calendar added = new()
        {
            Id = 0,
            Name = "Added"
        };

        Calendar updated = CreateCalendar();

        calendarServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newCalendar: added))
            .Returns(value: ValueTask.FromResult(result: added));

        calendarServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { updated }.AsQueryable());

        calendarServiceMock
            .Setup(expression: service => service.UpdateCalendarAsync(
                updatedCalendar: updated))
            .Returns(value: ValueTask.FromResult(result: updated));

        // When
        Result<Calendar>[] results = (await processingService
            .AddOrUpdateCalendar(items: new[] { added, updated }))
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

        calendarServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateCalendarFailure()
    {
        // Given

        Calendar item = new()
        {
            Id = 0,
            Name = "Failed"
        };

        calendarServiceMock
            .Setup(expression: service => service.AddCalendarAsync(
                newCalendar: item))
            .Throws(exception: new Exception("failed"));

        // When
        Result<Calendar> result = (await processingService
            .AddOrUpdateCalendar(items: new[] { item }))
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