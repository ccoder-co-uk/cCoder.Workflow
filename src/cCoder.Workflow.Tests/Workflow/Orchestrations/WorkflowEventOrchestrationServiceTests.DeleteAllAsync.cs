// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
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

        workflowEventProcessingServiceMock.Setup(expression: x => x.DeleteAllWorkflowEventAsync(deletedItems: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllWorkflowEventAsync(deletedItems: entities);

        // Then
        workflowEventProcessingServiceMock.Verify(expression: x => x.DeleteAllWorkflowEventAsync(deletedItems: entities), times: Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}