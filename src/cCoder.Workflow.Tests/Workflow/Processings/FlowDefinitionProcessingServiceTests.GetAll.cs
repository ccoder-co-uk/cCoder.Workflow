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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<FlowDefinition> entities = new[] { CreateRandomFlowDefinition() }.AsQueryable();
        flowDefinitionServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<FlowDefinition> result = flowDefinitionProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: entities);
        flowDefinitionServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

}