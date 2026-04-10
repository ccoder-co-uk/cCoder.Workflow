using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
    [Fact]
    public async Task ShouldAddOrUpdateChildInstancesWhenHandleFlowDefinitionAdd()
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition();

        flowInstanceDataOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(flowDefinition.Instances))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandleFlowDefinitionAddAsync(flowDefinition);

        // Then
        flowInstanceDataOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(flowDefinition.Instances),
            Times.Once
        );

        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}






