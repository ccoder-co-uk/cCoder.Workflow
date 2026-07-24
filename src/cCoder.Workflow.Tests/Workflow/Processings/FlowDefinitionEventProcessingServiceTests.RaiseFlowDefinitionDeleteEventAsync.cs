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
    public async Task ShouldPassThroughCallWhenRaiseFlowDefinitionDeleteEventAsync()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionEventServiceMock
            .Setup(x => x.RaiseFlowDefinitionDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowDefinitionDeleteEventAsync(entity);

        // Then
        flowDefinitionEventServiceMock.Verify(x => x.RaiseFlowDefinitionDeleteEventAsync(entity), Times.Once);
        flowDefinitionEventServiceMock.VerifyNoOtherCalls();
    }

}