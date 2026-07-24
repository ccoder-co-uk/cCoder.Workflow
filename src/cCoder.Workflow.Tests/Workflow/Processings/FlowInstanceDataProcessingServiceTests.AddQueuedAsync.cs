// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddQueuedAsync()
    {
        FlowInstanceData entity = CreateRandomFlowInstanceData();

        flowInstanceDataServiceMock
            .Setup(x => x.AddQueuedAsync(entity))
            .ReturnsAsync(entity);

        FlowInstanceData result = await flowInstanceDataProcessingService.AddQueuedAsync(entity);

        result.Should().BeSameAs(entity);
        flowInstanceDataServiceMock.Verify(x => x.AddQueuedAsync(entity), Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }
}