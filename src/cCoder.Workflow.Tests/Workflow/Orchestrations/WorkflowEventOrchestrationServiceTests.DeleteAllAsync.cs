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
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        WorkflowEvent[] entities = [CreateRandomWorkflowEvent()];
        workflowEventProcessingServiceMock.Setup(expression: x => x.DeleteAllAsync(items: entities)).Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(items: entities);

        // Then
        workflowEventProcessingServiceMock.Verify(expression: x => x.DeleteAllAsync(items: entities), times: Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}