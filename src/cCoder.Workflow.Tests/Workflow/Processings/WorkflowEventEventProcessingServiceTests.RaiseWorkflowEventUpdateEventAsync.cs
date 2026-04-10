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
            .Setup(x => x.RaiseWorkflowEventUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventUpdateEventAsync(entity);

        // Then
        workflowEventEventServiceMock.Verify(x => x.RaiseWorkflowEventUpdateEventAsync(entity), Times.Once);
        workflowEventEventServiceMock.VerifyNoOtherCalls();
    }

}







