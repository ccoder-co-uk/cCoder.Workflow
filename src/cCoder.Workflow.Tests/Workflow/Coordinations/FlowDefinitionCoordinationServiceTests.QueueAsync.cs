// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
    [Fact]
    public async Task QueueAsync_ShouldReturnQueuedFlowDefinitionId()
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();
        Guid queuedFlowDefinitionId = Guid.NewGuid();

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

        string args = "{}";

        flowQueueOrchestrationServiceMock
            .Setup(expression: service => service.QueueFlowDefinitionAsync(
                flowDefinitionId: flowDefinitionId,
                asUserId: asUserId,
                args: args))
            .ReturnsAsync(value: queuedFlowDefinitionId);

        // When
        Guid result = await coordinationService.QueueAsync(
            flowDefinitionId: flowDefinitionId,
            asUserId: asUserId,
            args: args);

        // Then
        result.Should()
            .Be(expected: queuedFlowDefinitionId);

        flowQueueOrchestrationServiceMock.Verify(
            expression: service => service.QueueFlowDefinitionAsync(
                flowDefinitionId: flowDefinitionId,
                asUserId: asUserId,
                args: args),
            times: Times.Once);

        flowQueueOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}