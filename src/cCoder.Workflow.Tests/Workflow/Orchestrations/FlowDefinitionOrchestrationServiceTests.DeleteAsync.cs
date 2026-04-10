using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowDefinition entity = CreateRandomFlowDefinition();
        entity.Id = id;
        flowDefinitionProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        flowDefinitionProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        flowDefinitionEventProcessingServiceMock
            .Setup(x => x.RaiseFlowDefinitionDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        flowDefinitionProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        flowDefinitionProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(x => x.RaiseFlowDefinitionDeleteEventAsync(entity), Times.Once);
    }

}







