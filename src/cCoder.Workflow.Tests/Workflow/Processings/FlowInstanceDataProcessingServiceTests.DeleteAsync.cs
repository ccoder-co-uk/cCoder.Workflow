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
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        var id = entity.Id;
        flowInstanceDataServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await flowInstanceDataProcessingService.DeleteAsync(id);

        // Then
        flowInstanceDataServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}