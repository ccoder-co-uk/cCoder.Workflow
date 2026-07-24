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
    public async Task ShouldPassThroughCallWhenRaiseFlowDefinitionAddEventAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionEventServiceMock
            .Setup(expression:x => x.RaiseFlowDefinitionAddEventAsync(entity))
            .Returns(value:ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionAddEventAsync(entity:entity);

        // Then
        flowDefinitionEventServiceMock.Verify(expression:x => x.RaiseFlowDefinitionAddEventAsync(entity), times:Times.Once);
        flowDefinitionEventServiceMock.VerifyNoOtherCalls();
    }

}