// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class WorkflowEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        workflowEventProcessingServiceMock.Setup(expression:x => x.UpdateAsync(entity)).ReturnsAsync(value:entity);

        workflowEventEventProcessingServiceMock
            .Setup(expression:x => x.RaiseWorkflowEventUpdateEventAsync(entity))
            .Returns(value:ValueTask.CompletedTask);

        // When
        WorkflowEvent result = await orchestrationService.UpdateAsync(entity:entity);

        // Then
        result.Should().BeSameAs(expected:entity);
        workflowEventProcessingServiceMock.Verify(expression:x => x.UpdateAsync(entity), times:Times.Once);
        workflowEventEventProcessingServiceMock.Verify(expression:x => x.RaiseWorkflowEventUpdateEventAsync(entity), times:Times.Once);
    }

}