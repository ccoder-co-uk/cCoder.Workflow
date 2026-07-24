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
    public async Task ShouldPassThroughCallWhenRaiseFlowInstanceDataAddEventAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataEventServiceMock
            .Setup(x => x.RaiseFlowInstanceDataAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFlowInstanceDataAddEventAsync(entity);

        // Then
        flowInstanceDataEventServiceMock.Verify(x => x.RaiseFlowInstanceDataAddEventAsync(entity), Times.Once);
        flowInstanceDataEventServiceMock.VerifyNoOtherCalls();
    }

}