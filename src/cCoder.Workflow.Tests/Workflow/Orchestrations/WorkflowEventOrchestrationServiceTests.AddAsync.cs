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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        workflowEventProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        workflowEventEventProcessingServiceMock
            .Setup(x => x.RaiseWorkflowEventAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        WorkflowEvent result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        workflowEventProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        workflowEventEventProcessingServiceMock.Verify(x => x.RaiseWorkflowEventAddEventAsync(entity), Times.Once);
    }

}