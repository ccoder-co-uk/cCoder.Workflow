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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<FlowInstanceData> entities = new[]
        {
            CreateRandomFlowInstanceData(),
        }.AsQueryable();
        flowInstanceDataServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<FlowInstanceData> result = flowInstanceDataProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        flowInstanceDataServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}







