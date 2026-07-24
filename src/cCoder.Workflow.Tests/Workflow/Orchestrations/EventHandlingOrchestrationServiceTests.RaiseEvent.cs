// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class EventHandlingOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldQueueAndRaiseFlowInstanceEventWhenMatchingSubscriptionExists()
    {
        Page page = CreateRandomPage();
        WorkflowEvent subscription = CreateSubscription(page: page);
        Guid queuedId = Guid.NewGuid();

        workflowEventProcessingServiceMock
            .Setup(expression: x => x.GetSubscriptionsAsync(appId: page.AppId, eventContext: $"page_update{page.Path}"))
            .ReturnsAsync(value: [subscription]);

        flowDefinitionCoordinationServiceMock
            .Setup(expression: x => x.QueueAsync(flowDefinitionId: subscription.FlowId, asUserId: subscription.ExecuteAs, args: It.IsAny<string>()))
            .ReturnsAsync(value: queuedId);

        await orchestrationService.RaiseEvents(payload: page, eventName: "page_update");

        workflowEventProcessingServiceMock.Verify(
expression: x => x.GetSubscriptionsAsync(appId: page.AppId, eventContext: $"page_update{page.Path}"),
times: Times.Once);

        flowDefinitionCoordinationServiceMock.Verify(
expression: x => x.QueueAsync(
flowDefinitionId: subscription.FlowId,
asUserId: subscription.ExecuteAs,
args: It.Is<string>(args => args.Contains("\"Path\":\"home\""))),
times: Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldIgnoreEventsWithoutAnAppId()
    {
        PageInfo pageInfo = new() { PageId = 1, CultureId = "en-GB" };

        await orchestrationService.RaiseEvents(payload: pageInfo, eventName: "page_info_update");

        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }
}