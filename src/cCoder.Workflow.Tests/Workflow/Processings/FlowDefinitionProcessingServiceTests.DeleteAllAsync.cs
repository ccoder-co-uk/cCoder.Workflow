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

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseDeleteAsyncPerItemIdWhenDeleteAllAsync()
    {
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionServiceMock
            .Setup(expression:x => x.DeleteWithInstancesAsync(entity.Id))
            .Returns(value:ValueTask.CompletedTask);

        await flowDefinitionProcessingService.DeleteAllAsync(items:new[] { entity });

        flowDefinitionServiceMock.Verify(expression:x => x.DeleteWithInstancesAsync(entity.Id), times:Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

}