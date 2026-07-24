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

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        var id = entity.Id;
        flowInstanceDataServiceMock.Setup(expression: x => x.DeleteAsync(flowInstanceDataId: id)).Returns(value: ValueTask.CompletedTask);

        // When
        await flowInstanceDataProcessingService.DeleteAllAsync(items: new[] { entity });

        // Then
        flowInstanceDataServiceMock.Verify(expression: x => x.DeleteAsync(flowInstanceDataId: id), times: Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}