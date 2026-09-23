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
    public async Task ShouldDelegateSubscriptionFilteringByAppAndContextAsync()
    {
        // Given
        WorkflowEvent matchingEvent = CreateRandomWorkflowEvent();
        matchingEvent.EventContext = "page_update/home";
        matchingEvent.Flow = new FlowDefinition { AppId = 1 };

        workflowEventServiceMock
            .Setup(expression: service => service.GetSubscriptions(
                appId: 1,
                eventContext: "page_update/home"))
            .Returns(value: [matchingEvent]);

        workflowEventServiceMock
            .Setup(expression: service => service
                .LogWorkflowEventSubscriptionsFound(count: 1))
            .Returns(value: true);

        // When
        WorkflowEvent[] result = await workflowEventProcessingService.GetSubscriptionsAsync(
            appId: 1,
            eventContext: "page_update/home");

        // Then
        result
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: matchingEvent);

        workflowEventServiceMock.Verify(
            expression: service => service.GetSubscriptions(
                appId: 1,
                eventContext: "page_update/home"),
            times: Times.Once);

        workflowEventServiceMock.Verify(
            expression: service => service
                .LogWorkflowEventSubscriptionsFound(count: 1),
            times: Times.Once);

        workflowEventServiceMock.VerifyNoOtherCalls();
    }
}