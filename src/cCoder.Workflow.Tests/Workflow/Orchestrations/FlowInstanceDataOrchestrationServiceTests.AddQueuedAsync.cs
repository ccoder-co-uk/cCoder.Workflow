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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddQueuedAsync()
    {
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock
            .Setup(x => x.AddQueuedAsync(entity))
            .ReturnsAsync(entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(x => x.RaiseFlowInstanceDataAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        FlowInstanceData result = await orchestrationService.AddQueuedAsync(entity);

        result.Should().BeSameAs(entity);
        flowInstanceDataProcessingServiceMock.Verify(x => x.AddQueuedAsync(entity), Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(x => x.RaiseFlowInstanceDataAddEventAsync(entity), Times.Once);
    }
}
