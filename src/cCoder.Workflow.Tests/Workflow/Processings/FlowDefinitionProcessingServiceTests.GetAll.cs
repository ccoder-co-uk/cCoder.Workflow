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
        flowDefinitionServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<FlowDefinition> result = flowDefinitionProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

}