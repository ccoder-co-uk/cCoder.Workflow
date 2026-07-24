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
        flowInstanceDataProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        flowInstanceDataProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        flowInstanceDataEventProcessingServiceMock
            .Setup(x => x.RaiseFlowInstanceDataDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        flowInstanceDataProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        flowInstanceDataProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        flowInstanceDataEventProcessingServiceMock.Verify(x => x.RaiseFlowInstanceDataDeleteEventAsync(entity), Times.Once);
    }

}