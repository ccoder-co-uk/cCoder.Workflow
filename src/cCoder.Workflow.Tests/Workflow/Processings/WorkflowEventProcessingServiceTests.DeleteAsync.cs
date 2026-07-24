// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        workflowEventServiceMock.Setup(expression:x => x.DeleteAsync(id)).Returns(value:ValueTask.CompletedTask);

        // When
        await workflowEventProcessingService.DeleteAsync(id:id);

        // Then
        workflowEventServiceMock.Verify(expression:x => x.DeleteAsync(id), times:Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}