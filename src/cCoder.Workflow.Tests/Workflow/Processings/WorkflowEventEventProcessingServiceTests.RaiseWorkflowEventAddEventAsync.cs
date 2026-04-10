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
    public async Task ShouldPassThroughCallWhenRaiseWorkflowEventAddEventAsync()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        workflowEventEventServiceMock
            .Setup(x => x.RaiseWorkflowEventAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventAddEventAsync(entity);

        // Then
        workflowEventEventServiceMock.Verify(x => x.RaiseWorkflowEventAddEventAsync(entity), Times.Once);
        workflowEventEventServiceMock.VerifyNoOtherCalls();
    }

}







