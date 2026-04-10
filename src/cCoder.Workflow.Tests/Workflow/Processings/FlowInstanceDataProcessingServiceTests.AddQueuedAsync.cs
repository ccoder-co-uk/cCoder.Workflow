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
    public async Task ShouldDelegateToBrokerWhenAddQueuedAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataBrokerMock
            .Setup(x => x.AddFlowInstanceDataAsync(
                It.Is<FlowInstanceData>(queued =>
                    queued.Id == entity.Id
                    && queued.FlowDefinitionId == entity.FlowDefinitionId
                    && queued.Name == entity.Name
                    && queued.ContextString == entity.ContextString
                    && queued.State == entity.State
                    && queued.ReportingComponentName == entity.ReportingComponentName
                    && queued.Caller == entity.Caller
                    && queued.Start == entity.Start
                    && queued.End == entity.End
                    && queued.FlowDefinition == null)))
            .ReturnsAsync(entity);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.AddQueuedAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataBrokerMock.Verify(
            x => x.AddFlowInstanceDataAsync(It.IsAny<FlowInstanceData>()),
            Times.Once);
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }
}
