// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

#pragma warning disable STXFORMAT009
public partial class FlowDefinitionAggregationServiceTests
{
    [Fact]
    public async Task ShouldPreserveMissingCallerForQueueCoordination()
    {
        // Given
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();
        flowDefinitionCoordinationServiceMock
            .Setup(expression: service => service.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: null,
                args: "{}"))
            .ReturnsAsync(value: queuedId);

        // When
        Guid result = await service.QueueFlowDefinitionAsync(
            flowDefinitionId: flowId,
            asUserId: null,
            args: "{}");

        // Then
        result.Should()
            .Be(expected: queuedId);


        flowDefinitionCoordinationServiceMock.Verify(
            expression: foundService => foundService.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: null,
                args: "{}"),
            times: Times.Once);

        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldQueueWithProvidedCallerIdWhenCallerIsPresent()
    {
        // Given
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();

        flowDefinitionCoordinationServiceMock
            .Setup(expression: service => service.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: "ash",
                args: "{}"))
            .ReturnsAsync(value: queuedId);

        // When
        Guid result = await service.QueueFlowDefinitionAsync(
            flowDefinitionId: flowId,
            asUserId: "ash",
            args: "{}");

        // Then
        result.Should()
            .Be(expected: queuedId);


        flowDefinitionCoordinationServiceMock.Verify(
            expression: foundService => foundService.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: "ash",
                args: "{}"),
            times: Times.Once);

        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldQueueAsGuestWhenCurrentUserIsMissing()
    {
        // Given
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();

        flowDefinitionCoordinationServiceMock
            .Setup(expression: foundService => foundService.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: "Guest",
                args: "{}"))
            .ReturnsAsync(value: queuedId);

        // When
        Guid result = await service.QueueFlowDefinitionAsync(
            flowDefinitionId: flowId,
            asUserId: "Guest",
            args: "{}");

        // Then
        result.Should().Be(expected: queuedId);
        flowDefinitionCoordinationServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT009