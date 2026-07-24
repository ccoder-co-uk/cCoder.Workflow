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

        workflowEventServiceMock.Setup(expression: x => x.DeleteAsync(workflowEventId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await workflowEventProcessingService.DeleteAsync(workflowEventId: id);

        // Then
        workflowEventServiceMock.Verify(expression: x => x.DeleteAsync(workflowEventId: id), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}