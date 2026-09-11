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
    public async Task ShouldPassThroughCallWhenRaiseWorkflowEventDeleteEventAsync()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();

        workflowEventEventServiceMock
            .Setup(expression: x => x.RaiseWorkflowEventDeleteEventAsync(workflowEvent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventDeleteEventAsync(workflowEvent: entity);

        // Then
        workflowEventEventServiceMock.Verify(expression: x => x.RaiseWorkflowEventDeleteEventAsync(workflowEvent: entity), times: Times.Once);
        workflowEventEventServiceMock.VerifyNoOtherCalls();
    }

}