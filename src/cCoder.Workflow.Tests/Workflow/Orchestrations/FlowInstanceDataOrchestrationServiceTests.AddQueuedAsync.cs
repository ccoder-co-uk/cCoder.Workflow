// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowInstanceDataOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddQueuedAsync()
    {
        FlowInstanceData entity = CreateRandomFlowInstanceData();

        flowInstanceDataProcessingServiceMock
            .Setup(expression: x => x.AddQueuedFlowInstanceDataAsync(newEntity: entity))
            .ReturnsAsync(value: entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowInstanceDataAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        FlowInstanceData result = await orchestrationService.AddQueuedFlowInstanceDataAsync(newEntity: entity);

        result.Should()
            .BeSameAs(expected: entity);

        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.AddQueuedFlowInstanceDataAsync(newEntity: entity), times: Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowInstanceDataAddEventAsync(entity: entity), times: Times.Once);
    }
}