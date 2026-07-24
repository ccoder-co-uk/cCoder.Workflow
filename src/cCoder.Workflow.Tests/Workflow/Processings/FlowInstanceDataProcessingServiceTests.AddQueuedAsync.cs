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
            .Setup(expression:x => x.AddQueuedAsync(entity))
            .ReturnsAsync(value:entity);

        FlowInstanceData result = await flowInstanceDataProcessingService.AddQueuedAsync(entity:entity);

        result.Should().BeSameAs(expected:entity);
        flowInstanceDataServiceMock.Verify(expression:x => x.AddQueuedAsync(entity), times:Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }
}