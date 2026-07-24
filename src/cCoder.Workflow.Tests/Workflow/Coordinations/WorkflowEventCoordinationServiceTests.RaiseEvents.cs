// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class WorkflowEventCoordinationServiceTests
{
    [Fact]
    public async Task ShouldQueueAndRaiseFlowInstanceEventWhenMatchingSubscriptionExists()
    {
        // Given
        Page page = CreateRandomPage();
        WorkflowEvent subscription = CreateSubscription(page: page);
        Guid queuedId = Guid.NewGuid();

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.PrepareWorkflowEventDispatch(
                payload: page,
                eventName: "page_update",
                appIdOverride: null))
            .Returns(value: (page.AppId, $"page_update{page.Path}"));

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.GetWorkflowEventSubscriptionsAsync(
                appId: page.AppId,
                eventContext: $"page_update{page.Path}"))
            .ReturnsAsync(value: [subscription]);

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.SerializeWorkflowEventPayload(payload: page))
            .Returns(value: "{\"Path\":\"home\"}");

        flowQueueOrchestrationServiceMock
            .Setup(expression: x => x.QueueFlowDefinitionAsync(
                flowDefinitionId: subscription.FlowId,
                asUserId: subscription.ExecuteAs,
                args: It.IsAny<string>()))
            .ReturnsAsync(value: queuedId);

        // When
        await coordinationService.RaiseEvents(payload: page, eventName: "page_update");

        // Then
        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.PrepareWorkflowEventDispatch(
                payload: page,
                eventName: "page_update",
                appIdOverride: null),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.GetWorkflowEventSubscriptionsAsync(
                appId: page.AppId,
                eventContext: $"page_update{page.Path}"),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.SerializeWorkflowEventPayload(payload: page),
            times: Times.Once);

        flowQueueOrchestrationServiceMock.Verify(
            expression: x => x.QueueFlowDefinitionAsync(
                flowDefinitionId: subscription.FlowId,
                asUserId: subscription.ExecuteAs,
                args: It.Is<string>(match: args => args.Contains(value: "\"Path\":\"home\""))),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.VerifyNoOtherCalls();
        flowQueueOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldIgnoreEventsWithoutAnAppId()
    {
        // Given
        PageInfo pageInfo = new() { PageId = 1, CultureId = "en-GB" };

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.PrepareWorkflowEventDispatch(
                payload: pageInfo,
                eventName: "page_info_update",
                appIdOverride: null))
            .Returns(value: (null, "page_info_update"));

        // When
        await coordinationService.RaiseEvents(payload: pageInfo, eventName: "page_info_update");

        // Then
        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.PrepareWorkflowEventDispatch(
                payload: pageInfo,
                eventName: "page_info_update",
                appIdOverride: null),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.VerifyNoOtherCalls();
        flowQueueOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}