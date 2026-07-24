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

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldFilterSubscriptionsByAppAndContextWhenGetSubscriptionsAsync()
    {
        // Given
        WorkflowEvent matchingEvent = CreateRandomWorkflowEvent();
        matchingEvent.EventContext = "page_update/home";
        matchingEvent.Flow = new FlowDefinition { AppId = 1 };

        WorkflowEvent wrongContextEvent = CreateRandomWorkflowEvent();
        wrongContextEvent.EventContext = "page_update/other";
        wrongContextEvent.Flow = new FlowDefinition { AppId = 1 };

        WorkflowEvent wrongAppEvent = CreateRandomWorkflowEvent();
        wrongAppEvent.EventContext = "page_update/home";
        wrongAppEvent.Flow = new FlowDefinition { AppId = 2 };

        IQueryable<WorkflowEvent> entities = new[] { matchingEvent, wrongContextEvent, wrongAppEvent }.AsQueryable();
        workflowEventServiceMock.Setup(expression:x => x.GetAll(true)).Returns(value:entities);

        // When
        WorkflowEvent[] result = await workflowEventProcessingService.GetSubscriptionsAsync(
appId:            1,
eventContext:            "page_update/home");

        // Then
        result.Should().ContainSingle().Which.Should().BeSameAs(expected:matchingEvent);
        workflowEventServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }
}