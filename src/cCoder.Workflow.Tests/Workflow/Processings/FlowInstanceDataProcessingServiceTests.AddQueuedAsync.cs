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
        flowInstanceDataBrokerMock.Setup(x => x.AddFlowInstanceDataAsync(entity)).ReturnsAsync(entity);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.AddQueuedAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataBrokerMock.Verify(x => x.AddFlowInstanceDataAsync(entity), Times.Once);
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }
}
