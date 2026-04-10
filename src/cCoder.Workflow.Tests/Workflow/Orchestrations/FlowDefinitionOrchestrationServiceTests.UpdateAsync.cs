using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        flowDefinitionEventProcessingServiceMock
            .Setup(x => x.RaiseFlowDefinitionUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FlowDefinition result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowDefinitionProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(x => x.RaiseFlowDefinitionUpdateEventAsync(entity), Times.Once);
    }

}







