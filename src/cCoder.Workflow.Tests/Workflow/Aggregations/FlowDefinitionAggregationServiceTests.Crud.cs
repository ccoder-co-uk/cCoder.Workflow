// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Services.Orchestrations;
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

        serviceProviderBrokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: FlowDefinitionOperation.Crud))
            .Returns(value: flowDefinitionOrchestrationServiceMock.Object);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.Get(
                flowDefinitionId: item.Id))
            .Returns(value: item);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: false))
            .Returns(value: items);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newEntity: item))
            .Returns(value: ValueTask.FromResult(result: item));

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.UpdateFlowDefinitionAsync(
                updatedEntity: item))
            .Returns(value: ValueTask.FromResult(result: item));

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAsync(
                flowDefinitionId: item.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowDefinition actualGet = service.GetFlowDefinition(
            flowDefinitionId: item.Id);

        IQueryable<FlowDefinition> actualAll = service.GetAllFlowDefinitions();
        FlowDefinition actualAdd = await service.AddFlowDefinitionAsync(newEntity: item);
        FlowDefinition actualUpdate = await service.UpdateFlowDefinitionAsync(updatedEntity: item);
        await service.DeleteFlowDefinitionAsync(flowDefinitionId: item.Id);

        // Then
        actualGet.Should().BeSameAs(expected: item);
        actualAll.Should().BeSameAs(expected: items);
        actualAdd.Should().BeSameAs(expected: item);
        actualUpdate.Should().BeSameAs(expected: item);
        flowDefinitionOrchestrationServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009