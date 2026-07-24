// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Setup(expression: x =>
                x.RaiseWorkflowEventUpdateEventAsync(message: It.IsAny<EventMessage<WorkflowEvent>>())
            )
            .Callback<EventMessage<WorkflowEvent>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseWorkflowEventUpdateEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        workflowEventEventBrokerMock.Verify(
expression: x => x.RaiseWorkflowEventUpdateEventAsync(message: It.IsAny<EventMessage<WorkflowEvent>>()),
times: Times.Once
        );

        workflowEventEventBrokerMock.VerifyNoOtherCalls();
    }

}