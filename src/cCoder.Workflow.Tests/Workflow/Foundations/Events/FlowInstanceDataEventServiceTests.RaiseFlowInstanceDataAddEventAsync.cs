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
    public async Task ShouldMapAndCallBrokerWhenRaiseFlowInstanceDataAddEventAsync()
    {
        // Given
        FlowInstanceData entity = new() { End = default(DateTimeOffset) };
        EventMessage<FlowInstanceData> actualMessage = null;

        flowInstanceDataEventBrokerMock
            .Setup(x =>
                x.RaiseFlowInstanceDataAddEventAsync(It.IsAny<EventMessage<FlowInstanceData>>())
            )
            .Callback<EventMessage<FlowInstanceData>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowInstanceDataAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        flowInstanceDataEventBrokerMock.Verify(
            x => x.RaiseFlowInstanceDataAddEventAsync(It.IsAny<EventMessage<FlowInstanceData>>()),
            Times.Once
        );
        flowInstanceDataEventBrokerMock.VerifyNoOtherCalls();
    }

}