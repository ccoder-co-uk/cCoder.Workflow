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

        workflowEventProcessingServiceMock.Setup(expression: x => x.Get(workflowEventId: id))
            .Returns(value: entity);

        workflowEventProcessingServiceMock.Setup(expression: x => x.DeleteAsync(workflowEventId: id))
            .Returns(value: ValueTask.CompletedTask);

        workflowEventEventProcessingServiceMock
            .Setup(expression: x => x.RaiseWorkflowEventDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(workflowEventId: id);

        // Then
        workflowEventProcessingServiceMock.Verify(expression: x => x.Get(workflowEventId: id), times: Times.Once);
        workflowEventProcessingServiceMock.Verify(expression: x => x.DeleteAsync(workflowEventId: id), times: Times.Once);
        workflowEventEventProcessingServiceMock.Verify(expression: x => x.RaiseWorkflowEventDeleteEventAsync(entity: entity), times: Times.Once);
    }

}