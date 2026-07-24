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

public partial class FlowInstanceDataEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFlowInstanceDataUpdateEventAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataEventServiceMock
            .Setup(expression:x => x.RaiseFlowInstanceDataUpdateEventAsync(entity))
            .Returns(value:ValueTask.CompletedTask);

        // When
        await service.RaiseFlowInstanceDataUpdateEventAsync(entity:entity);

        // Then
        flowInstanceDataEventServiceMock.Verify(expression:x => x.RaiseFlowInstanceDataUpdateEventAsync(entity), times:Times.Once);
        flowInstanceDataEventServiceMock.VerifyNoOtherCalls();
    }

}