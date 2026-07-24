// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowDefinitionEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFlowDefinitionAddEventAsync()
    {
        // Given
        FlowDefinition entity = new();
        EventMessage<FlowDefinition> actualMessage = null;

        flowDefinitionEventBrokerMock
            .Setup(expression:x =>
                x.RaiseFlowDefinitionAddEventAsync(It.IsAny<EventMessage<FlowDefinition>>())
            )
            .Callback<EventMessage<FlowDefinition>>(action:message => actualMessage = message)
            .Returns(value:ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionAddEventAsync(entity:entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(expectation:entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected:CurrentUserId);
        flowDefinitionEventBrokerMock.Verify(
expression:            x => x.RaiseFlowDefinitionAddEventAsync(It.IsAny<EventMessage<FlowDefinition>>()),
times:            Times.Once
        );
        flowDefinitionEventBrokerMock.VerifyNoOtherCalls();
    }

}