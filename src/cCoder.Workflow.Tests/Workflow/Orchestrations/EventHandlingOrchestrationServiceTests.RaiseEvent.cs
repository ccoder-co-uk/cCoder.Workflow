using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
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
        // Given
        Page page = CreateRandomPage();
        WorkflowEvent subscription = CreateSubscription(page);
        FlowInstanceData savedFlowInstance = null;

        workflowEventProcessingServiceMock
            .Setup(x => x.GetSubscriptionsAsync(page.AppId, $"page_update{page.Path}"))
            .ReturnsAsync([subscription]);

        flowInstanceDataProcessingServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData entity) =>
            {
                savedFlowInstance = entity;
                return entity;
            });

        flowInstanceDataEventProcessingServiceMock
            .Setup(x => x.RaiseFlowInstanceDataAddEventAsync(It.IsAny<FlowInstanceData>()))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.RaiseEvents(page, "page_update");

        // Then
        savedFlowInstance.Should().NotBeNull();
        savedFlowInstance.FlowDefinitionId.Should().Be(subscription.FlowId);
        savedFlowInstance.Caller.Should().Be(subscription.ExecuteAs);
        savedFlowInstance.State.Should().Be("Queued");

        workflowEventProcessingServiceMock.Verify(
            x => x.GetSubscriptionsAsync(page.AppId, $"page_update{page.Path}"),
            Times.Once);

        flowInstanceDataProcessingServiceMock.Verify(
            x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()),
            Times.Once);

        flowInstanceDataEventProcessingServiceMock.Verify(
            x => x.RaiseFlowInstanceDataAddEventAsync(It.IsAny<FlowInstanceData>()),
            Times.Once);
    }

    [Fact]
    public async Task ShouldIgnoreEventsWithoutAnAppId()
    {
        // Given
        PageInfo pageInfo = new() { PageId = 1, CultureId = "en-GB" };

        // When
        await orchestrationService.RaiseEvents(pageInfo, "page_info_update");

        // Then
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}
