// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Data.Models.Security;
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

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

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
                newFlowInstanceData: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData flowInstanceData) =>
            {
                flowInstanceData.Id = queuedFlowInstanceDataId;
                return flowInstanceData;
            });

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: service => service.RaiseFlowInstanceDataAddEventAsync(
                flowInstanceData: It.Is<FlowInstanceData>(
                    match: flowInstanceData =>
                        flowInstanceData.Id == queuedFlowInstanceDataId)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Guid result = await orchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: asUserId,
            args: args);

        // Then
        result.Should()
            .Be(expected: queuedFlowInstanceDataId);


        flowInstanceDataProcessingServiceMock.Verify(
            expression: service => service.AddQueuedFlowInstanceDataAsync(
                newFlowInstanceData: It.Is<FlowInstanceData>(
                    match: flowInstanceData =>
                        flowInstanceData.FlowDefinitionId == flowDefinitionId
                        && flowInstanceData.Caller == asUserId
                        && flowInstanceData.State == "Queued")),
            times: Times.Once);

        flowDefinitionProcessingServiceMock.VerifyAll();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task QueueFlowDefinitionAsync_WhenCallerIsMissing_ShouldUseCurrentUser()
    {
        // Given
        string currentUserId = Guid.NewGuid()
            .ToString(format: "N");

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: new User { Id = currentUserId });

        SetupQueueDependencies(expectedCaller: currentUserId);

        // When
        _ = await orchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: null,
            args: "{}");

        // Then
        VerifyQueuedCaller(expectedCaller: currentUserId);
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task QueueFlowDefinitionAsync_WhenGuestHasNoCurrentUser_ShouldUseGuest()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: null);

        SetupQueueDependencies(expectedCaller: "Guest");

        // When
        _ = await orchestrationService.QueueFlowDefinitionAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: "Guest",
            args: "{}");

        // Then
        VerifyQueuedCaller(expectedCaller: "Guest");
        authorizationBrokerMock.VerifyAll();
    }

    private readonly Guid flowDefinitionId = Guid.NewGuid();

    private void SetupQueueDependencies(string expectedCaller)
    {
        FlowDefinition flowDefinition = CreateFlowDefinition(flowDefinitionId: flowDefinitionId);

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.AuthorizeFlowDefinitionExecution(
                userId: expectedCaller,
                appId: flowDefinition.AppId))
            .Returns(value: true);

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.ParseFlowDefinition(
                definitionJson: flowDefinition.DefinitionJson))
            .Returns(value: CreateFlow());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.ParseFlowDefinitionData(args: "{}"))
            .Returns(value: new object());

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.SerializeFlowDefinitionContext(
                context: It.IsAny<object>()))
            .Returns(value: "{}");

        flowInstanceDataProcessingServiceMock
            .Setup(expression: service => service.AddQueuedFlowInstanceDataAsync(
                newFlowInstanceData: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData flowInstanceData) => flowInstanceData);

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: service => service.RaiseFlowInstanceDataAddEventAsync(
                flowInstanceData: It.IsAny<FlowInstanceData>()))
            .Returns(value: ValueTask.CompletedTask);
    }

    private void VerifyQueuedCaller(string expectedCaller)
    {
        flowInstanceDataProcessingServiceMock.Verify(
            expression: service => service.AddQueuedFlowInstanceDataAsync(
                newFlowInstanceData: It.Is<FlowInstanceData>(
                    match: flowInstanceData => flowInstanceData.Caller == expectedCaller)),
            times: Times.Once);

        flowDefinitionProcessingServiceMock.VerifyAll();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyAll();
    }
}