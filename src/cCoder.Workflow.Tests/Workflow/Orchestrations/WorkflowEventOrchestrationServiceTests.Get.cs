using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class WorkflowEventOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        workflowEventProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        WorkflowEvent result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        workflowEventProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







