// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FlowDefinition entity = CreateRandomFlowDefinition();
        var id = entity.Id;
        flowDefinitionServiceMock.Setup(expression: x => x.Get(flowDefinitionId: id)).Returns(value: entity);

        // When
        FlowDefinition result = flowDefinitionProcessingService.Get(flowDefinitionId: id);

        // Then
        result.Should().BeSameAs(expected: entity);
        flowDefinitionServiceMock.Verify(expression: x => x.Get(flowDefinitionId: id), times: Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

}