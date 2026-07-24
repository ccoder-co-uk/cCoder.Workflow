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
        workflowEventServiceMock.Setup(expression: x => x.DeleteAsync(workflowEventId: id)).Returns(value: ValueTask.CompletedTask);

        // When
        await workflowEventProcessingService.DeleteAllAsync(items: new[] { workflowEvent });

        // Then
        workflowEventServiceMock.Verify(expression: x => x.DeleteAsync(workflowEventId: id), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}