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
        Page page = CreateRandomPage();
        WorkflowEvent subscription = CreateSubscription(page: page);
        Guid queuedId = Guid.NewGuid();

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.PrepareWorkflowEventDispatch(page, "page_update", null))
            .Returns(value: (page.AppId, $"page_update{page.Path}"));

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.GetWorkflowEventSubscriptionsAsync(
                appId: page.AppId,
                eventContext: $"page_update{page.Path}"))
            .ReturnsAsync(value: [subscription]);

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.SerializeWorkflowEventPayload(page))
            .Returns(value: "{\"Path\":\"home\"}");

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.QueueFlowDefinitionAsync(
                flowDefinitionId: subscription.FlowId,
                asUserId: subscription.ExecuteAs,
                args: It.IsAny<string>()))
            .ReturnsAsync(value: queuedId);

        await coordinationService.RaiseEvents(payload: page, eventName: "page_update");

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.PrepareWorkflowEventDispatch(page, "page_update", null),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.GetWorkflowEventSubscriptionsAsync(
                appId: page.AppId,
                eventContext: $"page_update{page.Path}"),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.SerializeWorkflowEventPayload(page),
            times: Times.Once);

        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.QueueFlowDefinitionAsync(
                flowDefinitionId: subscription.FlowId,
                asUserId: subscription.ExecuteAs,
                args: It.Is<string>(args => args.Contains("\"Path\":\"home\""))),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.VerifyNoOtherCalls();
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldIgnoreEventsWithoutAnAppId()
    {
        PageInfo pageInfo = new() { PageId = 1, CultureId = "en-GB" };

        workflowEventOrchestrationServiceMock
            .Setup(expression: x => x.PrepareWorkflowEventDispatch(pageInfo, "page_info_update", null))
            .Returns(value: (null, "page_info_update"));

        await coordinationService.RaiseEvents(payload: pageInfo, eventName: "page_info_update");

        workflowEventOrchestrationServiceMock.Verify(
            expression: x => x.PrepareWorkflowEventDispatch(pageInfo, "page_info_update", null),
            times: Times.Once);

        workflowEventOrchestrationServiceMock.VerifyNoOtherCalls();
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
