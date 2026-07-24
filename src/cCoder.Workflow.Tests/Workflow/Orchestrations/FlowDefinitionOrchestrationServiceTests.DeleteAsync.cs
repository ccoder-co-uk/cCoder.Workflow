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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowDefinition entity = CreateRandomFlowDefinition();
        entity.Id = id;
        flowDefinitionProcessingServiceMock.Setup(expression:x => x.GetAll(true)).Returns(value:new[] { entity }.AsQueryable());
        flowDefinitionProcessingServiceMock.Setup(expression:x => x.DeleteAsync(id)).Returns(value:ValueTask.CompletedTask);

        flowDefinitionEventProcessingServiceMock
            .Setup(expression:x => x.RaiseFlowDefinitionDeleteEventAsync(entity))
            .Returns(value:ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id:id);

        // Then
        flowDefinitionProcessingServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        flowDefinitionProcessingServiceMock.Verify(expression:x => x.DeleteAsync(id), times:Times.Once);
        flowDefinitionEventProcessingServiceMock.Verify(expression:x => x.RaiseFlowDefinitionDeleteEventAsync(entity), times:Times.Once);
    }

}