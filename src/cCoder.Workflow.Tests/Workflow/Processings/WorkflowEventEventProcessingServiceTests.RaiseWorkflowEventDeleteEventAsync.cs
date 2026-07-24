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
            .Setup(x => x.RaiseWorkflowEventDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventDeleteEventAsync(entity);

        // Then
        workflowEventEventServiceMock.Verify(x => x.RaiseWorkflowEventDeleteEventAsync(entity), Times.Once);
        workflowEventEventServiceMock.VerifyNoOtherCalls();
    }

}