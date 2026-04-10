using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        var id = entity.Id;
        workflowEventServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        WorkflowEvent result = workflowEventProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        workflowEventServiceMock.Verify(x => x.Get(id), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}







