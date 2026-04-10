using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFlowDefinitionUpdateEventAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionEventServiceMock
            .Setup(x => x.RaiseFlowDefinitionUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionUpdateEventAsync(entity);

        // Then
        flowDefinitionEventServiceMock.Verify(x => x.RaiseFlowDefinitionUpdateEventAsync(entity), Times.Once);
        flowDefinitionEventServiceMock.VerifyNoOtherCalls();
    }

}







