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
        flowDefinitionServiceMock.Setup(x => x.UpdateAsync(flow)).ReturnsAsync(flow);

        // When
        FlowDefinition result = await flowDefinitionProcessingService.UpdateAsync(flow);

        // Then
        Assert.Same(flow, result);
        flowDefinitionServiceMock.Verify(x => x.UpdateAsync(flow), Times.Once);
    }

}