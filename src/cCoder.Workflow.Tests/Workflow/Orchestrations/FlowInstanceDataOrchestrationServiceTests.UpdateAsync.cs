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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(x => x.RaiseFlowInstanceDataUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FlowInstanceData result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(x => x.RaiseFlowInstanceDataUpdateEventAsync(entity), Times.Once);
    }

}







