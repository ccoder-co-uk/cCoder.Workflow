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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(x => x.RaiseFlowInstanceDataAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FlowInstanceData result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(x => x.RaiseFlowInstanceDataAddEventAsync(entity), Times.Once);
    }

}







