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
        WorkflowEvent subscription = CreateSubscription(page);
        Guid queuedId = Guid.NewGuid();

        workflowEventProcessingServiceMock
            .Setup(x => x.GetSubscriptionsAsync(page.AppId, $"page_update{page.Path}"))
            .ReturnsAsync([subscription]);

        flowDefinitionCoordinationServiceMock
            .Setup(x => x.QueueAsync(subscription.FlowId, subscription.ExecuteAs, It.IsAny<string>()))
            .ReturnsAsync(queuedId);

        await orchestrationService.RaiseEvents(page, "page_update");

        workflowEventProcessingServiceMock.Verify(
            x => x.GetSubscriptionsAsync(page.AppId, $"page_update{page.Path}"),
            Times.Once);

        flowDefinitionCoordinationServiceMock.Verify(
            x => x.QueueAsync(
                subscription.FlowId,
                subscription.ExecuteAs,
                It.Is<string>(args => args.Contains("\"Path\":\"home\""))),
            Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldIgnoreEventsWithoutAnAppId()
    {
        PageInfo pageInfo = new() { PageId = 1, CultureId = "en-GB" };

        await orchestrationService.RaiseEvents(pageInfo, "page_info_update");

        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
    }
}