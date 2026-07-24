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
            .With(func: item => item.FlowDefinitionId = flowDefinition.Id)
            .Build();

        IQueryable<FlowInstanceData> flowInstances = new[] { flowInstanceData }.AsQueryable();

        flowInstanceDataOrchestrationServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: flowInstances);

        flowInstanceDataOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllFlowInstanceDataAsync(deletedItems: It.IsAny<IEnumerable<FlowInstanceData>>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandleFlowDefinitionDeleteAsync(flowDefinition: flowDefinition);

        // Then
        flowInstanceDataOrchestrationServiceMock.Verify(expression: service => service.GetAll(ignoreFilters: true), times: Times.Once);

        flowInstanceDataOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllFlowInstanceDataAsync(
deletedItems: It.Is<IEnumerable<FlowInstanceData>>(match: items =>
                    items.Single().FlowDefinitionId == flowDefinition.Id
                )
            ),
times: Times.Once
        );

        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}