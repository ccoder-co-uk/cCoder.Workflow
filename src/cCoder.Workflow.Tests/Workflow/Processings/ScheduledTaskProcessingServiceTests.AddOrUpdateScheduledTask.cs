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
public partial class ScheduledTaskProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateScheduledTasks()
    {
        // Given

        ScheduledTask added = new()
        {
            Id = 0,
            Name = "Added"
        };

        ScheduledTask updated = CreateScheduledTask();

        scheduledTaskServiceMock
            .Setup(expression: service => service.AddScheduledTaskAsync(
                newScheduledTask: added))
            .Returns(value: ValueTask.FromResult(result: added));

        scheduledTaskServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { updated }
                .AsQueryable());

        scheduledTaskServiceMock
            .Setup(expression: service => service.UpdateScheduledTaskAsync(
                updatedScheduledTask: updated))
            .Returns(value: ValueTask.FromResult(result: updated));

        // When
        Result<ScheduledTask>[] results = (await processingService
            .AddOrUpdateScheduledTask(items: new[] { added, updated }))
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

        scheduledTaskServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateScheduledTaskFailure()
    {
        // Given

        ScheduledTask item = new()
        {
            Id = 0,
            Name = "Failed"
        };

        scheduledTaskServiceMock
            .Setup(expression: service => service.AddScheduledTaskAsync(
                newScheduledTask: item))
            .Throws(exception: new Exception("failed"));

        // When
        Result<ScheduledTask> result = (await processingService
            .AddOrUpdateScheduledTask(items: new[] { item }))
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