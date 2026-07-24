// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class WorkflowEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        workflowEventProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        workflowEventProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        workflowEventEventProcessingServiceMock
            .Setup(x => x.RaiseWorkflowEventDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        workflowEventProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        workflowEventProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        workflowEventEventProcessingServiceMock.Verify(x => x.RaiseWorkflowEventDeleteEventAsync(entity), Times.Once);
    }

}