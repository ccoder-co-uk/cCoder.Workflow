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


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowDefinitionServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        cCoder.Data.Models.Workflow.FlowDefinition flowDefinition =
            CreateRandomFlowDefinition();
        IQueryable<cCoder.Data.Models.Workflow.FlowDefinition> flowDefinitions = new[]
        {
            flowDefinition
        }.AsQueryable();

        flowDefinitionBrokerMock
            .Setup(x => x.GetAllFlowDefinitions(false))
            .Returns(flowDefinitions);

        // When
        IQueryable<FlowDefinition> result = flowDefinitionService.GetAll();

        // Then
        result.Should().BeEquivalentTo(flowDefinitions.Select(item => (FlowDefinition)item));
        flowDefinitionBrokerMock.Verify(x => x.GetAllFlowDefinitions(false), Times.Once);
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<cCoder.Data.Models.Workflow.FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}