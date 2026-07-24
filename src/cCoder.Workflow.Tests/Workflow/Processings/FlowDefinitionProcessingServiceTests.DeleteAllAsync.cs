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
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();

        flowDefinitionServiceMock
            .Setup(expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: entity.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await flowDefinitionProcessingService.DeleteAllFlowDefinitionAsync(deletedItems: new[] { entity });

        // Then
        flowDefinitionServiceMock.Verify(expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: entity.Id), times: Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

}