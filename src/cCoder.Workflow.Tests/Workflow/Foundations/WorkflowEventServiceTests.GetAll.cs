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

        workflowEventBrokerMock.Setup(expression: x => x.GetAllWorkflowEvents(ignoreFilters: false))
            .Returns(value: workflowEvents);

        // When
        IQueryable<WorkflowEvent> result = workflowEventService.GetAll();

        // Then
        result.Should()
            .BeEquivalentTo(expectation: workflowEvents.Select(selector: item => (WorkflowEvent)item));

        workflowEventBrokerMock.Verify(expression: x => x.GetAllWorkflowEvents(ignoreFilters: false), times: Times.Once);

        workflowEventBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Workflow.WorkflowEvent>()),
times: Times.AtMostOnce()
        );

        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}