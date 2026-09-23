// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT009
public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateWorkflowEventsAsync()
    {
        // Given
        WorkflowEvent added = CreateRandomWorkflowEvent();
        added.Id = Guid.Empty;
        WorkflowEvent updated = CreateRandomWorkflowEvent();

        foreach (WorkflowEvent item in new[] { added, updated })
        {
            workflowEventServiceMock
                .Setup(expression: service => service.AuthorizeWorkflowEvent(
                    workflowEvent: item))
                .Returns(value: true);
        }

        workflowEventServiceMock
            .Setup(expression: service => service.AddWorkflowEventAsync(
                newWorkflowEvent: added))
            .Returns(value: ValueTask.FromResult(result: added));

        workflowEventServiceMock
            .Setup(expression: service => service.UpdateWorkflowEventAsync(
                updatedWorkflowEvent: updated))
            .Returns(value: ValueTask.FromResult(result: updated));

        // When
        Result<WorkflowEvent>[] results = (await workflowEventProcessingService
            .AddOrUpdateWorkflowEvent(items: new[] { added, updated }))
            .ToArray();

        // Then
        results.Should().OnlyContain(predicate: result => result.Success);
        results[0].Message.Should().Be(expected: "Added Successfully");
        results[1].Message.Should().Be(expected: "Updated Successfully");
        workflowEventServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateWorkflowEventFailureAsync()
    {
        // Given
        WorkflowEvent item = CreateRandomWorkflowEvent();
        item.Id = Guid.Empty;

        workflowEventServiceMock
            .Setup(expression: service => service.AuthorizeWorkflowEvent(
                workflowEvent: item))
            .Throws(exception: new Exception(message: "failed"));

        // When
        Result<WorkflowEvent> result = (await workflowEventProcessingService
            .AddOrUpdateWorkflowEvent(items: new[] { item }))
            .Single();

        // Then
        result.Success.Should().BeFalse();
        result.Item.Should().BeSameAs(expected: item);
        result.Message.Should().Be(expected: "The Workflow service failed.");
    }
}
#pragma warning restore STXFORMAT009