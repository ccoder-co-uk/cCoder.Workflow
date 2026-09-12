// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseWorkflowEventUpdateEventAsync()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();

        workflowEventEventServiceMock
            .Setup(expression: x => x.RaiseWorkflowEventUpdateEventAsync(workflowEvent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventUpdateEventAsync(workflowEvent: entity);

        // Then
        workflowEventEventServiceMock.Verify(expression: x => x.RaiseWorkflowEventUpdateEventAsync(workflowEvent: entity), times: Times.Once);
        workflowEventEventServiceMock.VerifyNoOtherCalls();
    }

}