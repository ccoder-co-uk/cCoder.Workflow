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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.AddAsync(entity: entity)).ReturnsAsync(value: entity);

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowInstanceDataAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowInstanceData result = await orchestrationService.AddAsync(entity: entity);

        // Then
        result.Should().BeSameAs(expected: entity);
        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.AddAsync(entity: entity), times: Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowInstanceDataAddEventAsync(entity: entity), times: Times.Once);
    }

}