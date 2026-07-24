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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        FlowDefinition flow = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Flow",
        };

        flowDefinitionServiceMock.Setup(expression: x => x.UpdateFlowDefinitionAsync(updatedFlowDefinition: flow))
            .ReturnsAsync(value: flow);

        // When
        FlowDefinition result = await flowDefinitionProcessingService.UpdateFlowDefinitionAsync(updatedEntity: flow);

        // Then
        Assert.Same(expected: flow, actual: result);
        flowDefinitionServiceMock.Verify(expression: x => x.UpdateFlowDefinitionAsync(updatedFlowDefinition: flow), times: Times.Once);
    }

}