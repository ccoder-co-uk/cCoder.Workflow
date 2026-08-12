// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005
public partial class ScheduledTaskOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseDeleteEventBeforeDeletingScheduledTaskAsync()
    {
        // Given
        ScheduledTask task = CreateScheduledTask();

        processingServiceMock.Setup(expression: found => found.GetAll(true))
            .Returns(new[] { task }.AsQueryable());
        eventServiceMock.Setup(expression: found => found.RaiseScheduledTaskDeleteEventAsync(task))
            .Returns(value: ValueTask.CompletedTask);
        processingServiceMock.Setup(expression: found => found.DeleteAsync(task.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(scheduledTaskId: task.Id);

        // Then
        processingServiceMock.VerifyAll();
        eventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldIgnoreMissingScheduledTaskWhenDeletingAsync()
    {
        // Given
        processingServiceMock.Setup(expression: found => found.GetAll(true))
            .Returns(Array.Empty<ScheduledTask>().AsQueryable());

        // When
        await service.DeleteAsync(scheduledTaskId: 1);

        // Then
        processingServiceMock.VerifyAll();
        eventServiceMock.VerifyNoOtherCalls();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005