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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();

        flowDefinitionProcessingServiceMock.Setup(expression: x => x.UpdateFlowDefinitionAsync(updatedFlowDefinition: entity))
            .ReturnsAsync(value: entity);

        flowDefinitionEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowDefinitionUpdateEventAsync(flowDefinition: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FlowDefinition result = await orchestrationService.UpdateFlowDefinitionAsync(updatedFlowDefinition: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);


        flowDefinitionProcessingServiceMock.Verify(expression: x => x.UpdateFlowDefinitionAsync(updatedFlowDefinition: entity), times: Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowDefinitionUpdateEventAsync(flowDefinition: entity), times: Times.Once);
    }

}