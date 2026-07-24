// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowQueueOrchestrationServiceTests
{
    [Fact]
    public async Task QueueFlowDefinitionAsync_ShouldPersistQueuedFlowInstanceData()
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();
        Guid queuedFlowInstanceDataId = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString(format: "N");
        string args = "{}";
        FlowDefinition flowDefinition = CreateFlowDefinition(flowDefinitionId: flowDefinitionId);

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.AuthorizeFlowDefinitionExecution(
                userId: asUserId,
                appId: flowDefinition.AppId))
            .Returns(value: true);

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.ParseFlowDefinition(
                definitionJson: flowDefinition.DefinitionJson))
            .Returns(value: CreateFlow());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.ParseFlowDefinitionData(args: args))
            .Returns(value: new object());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.SerializeFlowDefinitionContext(
                context: It.IsAny<object>()))
            .Returns(value: "{}");

        flowInstanceDataProcessingServiceMock
            .Setup(expression: service => service.AddQueuedFlowInstanceDataAsync(
                newEntity: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData flowInstanceData) =>
            {
                flowInstanceData.Id = queuedFlowInstanceDataId;
                return flowInstanceData;
            });

        // When
        Guid result = await orchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: asUserId,
            args: args);

        // Then
        result.Should().Be(expected: queuedFlowInstanceDataId);

        flowInstanceDataProcessingServiceMock.Verify(
            expression: service => service.AddQueuedFlowInstanceDataAsync(
                newEntity: It.Is<FlowInstanceData>(
                    flowInstanceData =>
                        flowInstanceData.FlowDefinitionId == flowDefinitionId
                        && flowInstanceData.Caller == asUserId
                        && flowInstanceData.State == "Queued")),
            times: Times.Once);

        flowDefinitionProcessingServiceMock.VerifyAll();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
    }
}