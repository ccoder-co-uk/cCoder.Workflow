using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        var id = entity.Id;
        flowInstanceDataServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FlowInstanceData result = flowInstanceDataProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataServiceMock.Verify(x => x.Get(id), Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}







