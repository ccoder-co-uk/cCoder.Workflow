using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFlowInstanceDataUpdateEventAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataEventServiceMock
            .Setup(x => x.RaiseFlowInstanceDataUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowInstanceDataUpdateEventAsync(entity);

        // Then
        flowInstanceDataEventServiceMock.Verify(x => x.RaiseFlowInstanceDataUpdateEventAsync(entity), Times.Once);
        flowInstanceDataEventServiceMock.VerifyNoOtherCalls();
    }

}







