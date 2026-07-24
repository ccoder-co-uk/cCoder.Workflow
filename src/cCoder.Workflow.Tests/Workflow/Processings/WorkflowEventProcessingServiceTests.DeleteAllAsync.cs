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

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        Guid id = workflowEvent.Id;
        workflowEventServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await workflowEventProcessingService.DeleteAllAsync(new[] { workflowEvent });

        // Then
        workflowEventServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}