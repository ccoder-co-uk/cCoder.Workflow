using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class WorkflowEventEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseWorkflowEventUpdateEventAsync()
    {
        // Given
        WorkflowEvent entity = new();
        EventMessage<WorkflowEvent> actualMessage = null;

        workflowEventEventBrokerMock
            .Setup(x =>
                x.RaiseWorkflowEventUpdateEventAsync(It.IsAny<EventMessage<WorkflowEvent>>())
            )
            .Callback<EventMessage<WorkflowEvent>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        workflowEventEventBrokerMock.Verify(
            x => x.RaiseWorkflowEventUpdateEventAsync(It.IsAny<EventMessage<WorkflowEvent>>()),
            Times.Once
        );
        workflowEventEventBrokerMock.VerifyNoOtherCalls();
    }

}







