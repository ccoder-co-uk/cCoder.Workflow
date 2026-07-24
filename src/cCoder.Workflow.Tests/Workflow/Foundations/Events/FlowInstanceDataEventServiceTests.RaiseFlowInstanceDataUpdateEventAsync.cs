// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowInstanceDataEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFlowInstanceDataUpdateEventAsync()
    {
        // Given
        FlowInstanceData entity = new() { End = default(DateTimeOffset) };
        EventMessage<FlowInstanceData> actualMessage = null;

        flowInstanceDataEventBrokerMock
            .Setup(expression:x =>
                x.RaiseFlowInstanceDataUpdateEventAsync(It.IsAny<EventMessage<FlowInstanceData>>())
            )
            .Callback<EventMessage<FlowInstanceData>>(action:message => actualMessage = message)
            .Returns(value:ValueTask.CompletedTask);

        // When
        await service.RaiseFlowInstanceDataUpdateEventAsync(entity:entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(expectation:entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected:CurrentUserId);
        flowInstanceDataEventBrokerMock.Verify(
expression:            x =>
                x.RaiseFlowInstanceDataUpdateEventAsync(It.IsAny<EventMessage<FlowInstanceData>>()),
times:            Times.Once
        );
        flowInstanceDataEventBrokerMock.VerifyNoOtherCalls();
    }

}