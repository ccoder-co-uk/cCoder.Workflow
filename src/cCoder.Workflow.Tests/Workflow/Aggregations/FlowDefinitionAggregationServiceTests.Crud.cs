// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009
public partial class FlowDefinitionAggregationServiceTests
{
    [Fact]
    public async Task ShouldDelegateFlowDefinitionCrudOperationsAsync()
    {
        // Given
        FlowDefinition item = new() { Id = Guid.NewGuid() };
        IQueryable<FlowDefinition> items = new[] { item }.AsQueryable();

        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.GetFlowDefinition(
                flowDefinitionId: item.Id))
            .Returns(value: item);

        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.GetAllFlowDefinitions())
            .Returns(value: items);

        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: item))
            .Returns(value: ValueTask.FromResult(result: item));

        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.UpdateFlowDefinitionAsync(
                updatedFlowDefinition: item))
            .Returns(value: ValueTask.FromResult(result: item));

        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.DeleteFlowDefinitionAsync(
                flowDefinitionId: item.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowDefinition actualGet = service.GetFlowDefinition(
            flowDefinitionId: item.Id);

        IQueryable<FlowDefinition> actualAll = service.GetAllFlowDefinitions();
        FlowDefinition actualAdd = await service.AddFlowDefinitionAsync(newFlowDefinition: item);
        FlowDefinition actualUpdate = await service.UpdateFlowDefinitionAsync(updatedFlowDefinition: item);
        await service.DeleteFlowDefinitionAsync(flowDefinitionId: item.Id);

        // Then
        actualGet.Should().BeSameAs(expected: item);
        actualAll.Should().BeSameAs(expected: items);
        actualAdd.Should().BeSameAs(expected: item);
        actualUpdate.Should().BeSameAs(expected: item);
        flowDefinitionManagementCoordinationServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009