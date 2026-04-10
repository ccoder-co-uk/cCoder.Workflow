using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowInstanceDataOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<FlowInstanceData> entities = new[] { CreateRandomFlowInstanceData() }.AsQueryable();
        flowInstanceDataProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<FlowInstanceData> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        flowInstanceDataProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







