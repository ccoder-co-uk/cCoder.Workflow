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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        flowDefinitionEventProcessingServiceMock
            .Setup(x => x.RaiseFlowDefinitionAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FlowDefinition result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowDefinitionProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(x => x.RaiseFlowDefinitionAddEventAsync(entity), Times.Once);
    }

}







