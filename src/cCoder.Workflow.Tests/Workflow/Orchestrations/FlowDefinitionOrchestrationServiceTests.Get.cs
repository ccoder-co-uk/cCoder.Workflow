using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FlowDefinition result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        flowDefinitionProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







