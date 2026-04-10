using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        var id = entity.Id;
        flowDefinitionServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FlowDefinition result = flowDefinitionProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        flowDefinitionServiceMock.Verify(x => x.Get(id), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

}







