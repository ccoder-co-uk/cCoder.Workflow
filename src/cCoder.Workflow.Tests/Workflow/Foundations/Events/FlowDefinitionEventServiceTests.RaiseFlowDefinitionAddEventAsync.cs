using cCoder.Data.Models.Workflow;
using EventLibrary.Models;
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
            .Setup(x =>
                x.RaiseFlowDefinitionAddEventAsync(It.IsAny<EventMessage<FlowDefinition>>())
            )
            .Callback<EventMessage<FlowDefinition>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        flowDefinitionEventBrokerMock.Verify(
            x => x.RaiseFlowDefinitionAddEventAsync(It.IsAny<EventMessage<FlowDefinition>>()),
            Times.Once
        );
        flowDefinitionEventBrokerMock.VerifyNoOtherCalls();
    }

}







