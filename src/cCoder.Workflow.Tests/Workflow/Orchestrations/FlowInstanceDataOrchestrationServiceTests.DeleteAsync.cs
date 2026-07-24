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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.Get(id: id)).Returns(value: entity);
        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id)).Returns(value: ValueTask.CompletedTask);

        flowInstanceDataEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFlowInstanceDataDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id: id);

        // Then
        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(expression: x => x.RaiseFlowInstanceDataDeleteEventAsync(entity: entity), times: Times.Once);
    }

}