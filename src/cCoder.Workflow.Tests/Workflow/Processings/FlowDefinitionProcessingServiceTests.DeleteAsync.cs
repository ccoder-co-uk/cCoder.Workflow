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
            .Setup(expression:x => x.DeleteWithInstancesAsync(flowId))
            .Returns(value:ValueTask.CompletedTask);

        await flowDefinitionProcessingService.DeleteAsync(id:flowId);

        flowDefinitionServiceMock.Verify(
expression:            x => x.DeleteWithInstancesAsync(flowId),
times:            Times.Once
        );
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldBubbleExceptionFromServiceForDeleteAsync()
    {
        Guid flowId = Guid.NewGuid();
        flowDefinitionServiceMock
            .Setup(expression:x => x.DeleteWithInstancesAsync(flowId))
            .Returns(value:ValueTask.FromException(new InvalidOperationException("boom")));

        await Assert.ThrowsAsync<InvalidOperationException>(testCode:async () =>
            await flowDefinitionProcessingService.DeleteAsync(flowId)
        );

        flowDefinitionServiceMock.Verify(expression:x => x.DeleteWithInstancesAsync(flowId), times:Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }
}