// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(x => x.GetAllWorkflowEvents(false))
            .Returns(new[] { workflowEvent }.AsQueryable());

        // When
        WorkflowEvent result = workflowEventService.Get(workflowEvent.Id);

        // Then
        result.Should().BeEquivalentTo(workflowEvent);
        workflowEventBrokerMock.Verify(x => x.GetAllWorkflowEvents(false), Times.Once);
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}