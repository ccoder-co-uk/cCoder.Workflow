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

public partial class FlowInstanceDataOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        FlowInstanceData[] entities = [CreateRandomFlowInstanceData()];
        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.DeleteAllAsync(items: entities)).Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(items: entities);

        // Then
        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.DeleteAllAsync(items: entities), times: Times.Once);
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}