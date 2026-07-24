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
            .Setup(expression: x => x.AddQueuedFlowInstanceDataAsync(newFlowInstanceData: entity))
            .ReturnsAsync(value: entity);

        FlowInstanceData result = await flowInstanceDataProcessingService.AddQueuedFlowInstanceDataAsync(newEntity: entity);

        result.Should()
            .BeSameAs(expected: entity);

        flowInstanceDataServiceMock.Verify(expression: x => x.AddQueuedFlowInstanceDataAsync(newFlowInstanceData: entity), times: Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }
}