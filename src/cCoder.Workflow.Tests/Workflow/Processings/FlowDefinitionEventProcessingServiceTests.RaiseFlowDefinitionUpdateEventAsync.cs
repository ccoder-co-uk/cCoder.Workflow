// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFlowDefinitionUpdateEventAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionEventServiceMock
            .Setup(expression: x => x.RaiseFlowDefinitionUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionUpdateEventAsync(entity: entity);

        // Then
        flowDefinitionEventServiceMock.Verify(expression: x => x.RaiseFlowDefinitionUpdateEventAsync(entity: entity), times: Times.Once);
        flowDefinitionEventServiceMock.VerifyNoOtherCalls();
    }

}