// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        FlowDefinition[] entities = [CreateRandomFlowDefinition()];
        flowDefinitionProcessingServiceMock.Setup(expression:x => x.DeleteAllAsync(entities)).Returns(value:ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(items:entities);

        // Then
        flowDefinitionProcessingServiceMock.Verify(expression:x => x.DeleteAllAsync(entities), times:Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}