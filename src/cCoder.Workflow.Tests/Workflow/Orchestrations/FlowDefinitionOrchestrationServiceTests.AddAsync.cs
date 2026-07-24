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

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionProcessingServiceMock.Setup(expression: x => x.AddAsync(entity: entity))
            .ReturnsAsync(value: entity);

        flowDefinitionEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowDefinitionAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowDefinition result = await orchestrationService.AddAsync(entity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);
        flowDefinitionProcessingServiceMock.Verify(expression: x => x.AddAsync(entity: entity), times: Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowDefinitionAddEventAsync(entity: entity), times: Times.Once);
    }

}