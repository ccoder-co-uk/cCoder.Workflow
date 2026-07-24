// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        cCoder.Data.Models.Workflow.WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        IQueryable<cCoder.Data.Models.Workflow.WorkflowEvent> workflowEvents = new[]
        {
            workflowEvent
        }.AsQueryable();

        workflowEventBrokerMock.Setup(x => x.GetAllWorkflowEvents(false)).Returns(workflowEvents);

        // When
        IQueryable<WorkflowEvent> result = workflowEventService.GetAll();

        // Then
        result.Should().BeEquivalentTo(workflowEvents.Select(item => (WorkflowEvent)item));
        workflowEventBrokerMock.Verify(x => x.GetAllWorkflowEvents(false), Times.Once);
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<cCoder.Data.Models.Workflow.WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}