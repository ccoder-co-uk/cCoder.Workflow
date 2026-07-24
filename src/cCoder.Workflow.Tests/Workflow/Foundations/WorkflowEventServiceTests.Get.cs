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
            .Setup(expression: x => x.GetAllWorkflowEvents(ignoreFilters: false))
            .Returns(value: new[] { workflowEvent }.AsQueryable());

        // When
        WorkflowEvent result = workflowEventService.Get(id: workflowEvent.Id);

        // Then
        result.Should().BeEquivalentTo(expectation: workflowEvent);
        workflowEventBrokerMock.Verify(expression: x => x.GetAllWorkflowEvents(ignoreFilters: false), times: Times.Once);
        workflowEventBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<WorkflowEvent>()),
times: Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}