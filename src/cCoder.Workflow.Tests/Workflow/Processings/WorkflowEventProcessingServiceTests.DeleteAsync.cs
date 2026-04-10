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
        workflowEventServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await workflowEventProcessingService.DeleteAsync(id);

        // Then
        workflowEventServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}





