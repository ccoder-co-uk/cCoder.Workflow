// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToServiceForDeleteAsync()
    {
        Guid flowId = Guid.NewGuid();
        flowDefinitionServiceMock
            .Setup(x => x.DeleteWithInstancesAsync(flowId))
            .Returns(ValueTask.CompletedTask);

        await flowDefinitionProcessingService.DeleteAsync(flowId);

        flowDefinitionServiceMock.Verify(
            x => x.DeleteWithInstancesAsync(flowId),
            Times.Once
        );
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldBubbleExceptionFromServiceForDeleteAsync()
    {
        Guid flowId = Guid.NewGuid();
        flowDefinitionServiceMock
            .Setup(x => x.DeleteWithInstancesAsync(flowId))
            .Returns(ValueTask.FromException(new InvalidOperationException("boom")));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await flowDefinitionProcessingService.DeleteAsync(flowId)
        );

        flowDefinitionServiceMock.Verify(x => x.DeleteWithInstancesAsync(flowId), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }
}