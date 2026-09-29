// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFlowDefinitionAddEventAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();

        flowDefinitionEventServiceMock
            .Setup(expression: x => x.RaiseFlowDefinitionAddEventAsync(flowDefinition: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionAddEventAsync(flowDefinition: entity);

        // Then
        flowDefinitionEventServiceMock.Verify(expression: x => x.RaiseFlowDefinitionAddEventAsync(flowDefinition: entity), times: Times.Once);
        flowDefinitionEventServiceMock.VerifyNoOtherCalls();
    }

}