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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();

        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.UpdateFlowInstanceDataAsync(updatedEntity: entity))
            .ReturnsAsync(value: entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowInstanceDataUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowInstanceData result = await orchestrationService.UpdateFlowInstanceDataAsync(updatedEntity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.UpdateFlowInstanceDataAsync(updatedEntity: entity), times: Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowInstanceDataUpdateEventAsync(entity: entity), times: Times.Once);
    }

}