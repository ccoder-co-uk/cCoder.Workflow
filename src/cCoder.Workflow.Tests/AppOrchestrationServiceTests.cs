using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public class AppOrchestrationServiceTests
{
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly AppOrchestrationService service;

    public AppOrchestrationServiceTests()
    {
        flowDefinitionOrchestrationServiceMock = new Mock<IFlowDefinitionOrchestrationService>(MockBehavior.Strict);
        service = new AppOrchestrationService(flowDefinitionOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedFlowsByAppIdWhenDeleteAsync()
    {
        flowDefinitionOrchestrationServiceMock.Setup(x => x.DeleteByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);

        await service.DeleteAsync(5);

        flowDefinitionOrchestrationServiceMock.Verify(x => x.DeleteByAppIdAsync(5), Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldStampFlowAppIdsWhenAddAsync()
    {
        App app = new()
        {
            Id = 9,
            Flows = [new FlowDefinition { Id = Guid.NewGuid(), Name = "Flow" }]
        };

        flowDefinitionOrchestrationServiceMock.Setup(x => x.AddOrUpdate(
                It.Is<IEnumerable<FlowDefinition>>(items => items.All(flow => flow.AppId == 9))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.Workflow.Models.Result<FlowDefinition>>>([]));

        await service.AddAsync(app);

        flowDefinitionOrchestrationServiceMock.VerifyAll();
    }
}
