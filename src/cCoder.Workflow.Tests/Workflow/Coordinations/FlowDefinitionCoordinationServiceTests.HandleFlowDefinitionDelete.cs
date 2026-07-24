// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FizzWare.NBuilder;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
    [Fact]
    public async Task ShouldFetchAndDeleteChildInstancesWhenHandleFlowDefinitionDelete()
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition();

        FlowInstanceData flowInstanceData = Builder<FlowInstanceData>
            .CreateNew()
            .With(item => item.FlowDefinitionId = flowDefinition.Id)
            .Build();

        IQueryable<FlowInstanceData> flowInstances = new[] { flowInstanceData }.AsQueryable();

        flowInstanceDataOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(flowInstances);

        flowInstanceDataOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(It.IsAny<IEnumerable<FlowInstanceData>>()))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.HandleFlowDefinitionDeleteAsync(flowDefinition);

        // Then
        flowInstanceDataOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);

        flowInstanceDataOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(
                It.Is<IEnumerable<FlowInstanceData>>(items =>
                    items.Single().FlowDefinitionId == flowDefinition.Id
                )
            ),
            Times.Once
        );

        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}