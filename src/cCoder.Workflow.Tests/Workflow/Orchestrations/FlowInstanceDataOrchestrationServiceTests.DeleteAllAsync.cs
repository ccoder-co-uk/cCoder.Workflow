// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
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

        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.DeleteAllFlowInstanceDataAsync(deletedItems: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllFlowInstanceDataAsync(deletedItems: entities);

        // Then
        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.DeleteAllFlowInstanceDataAsync(deletedItems: entities), times: Times.Once);
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}