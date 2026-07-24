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
            .Setup(expression: x => x.GetAllFlowDefinitions(ignoreFilters: false))
            .Returns(value: flowDefinitions);

        // When
        IQueryable<FlowDefinition> result = flowDefinitionService.GetAll();

        // Then
        result.Should()
            .BeEquivalentTo(expectation: flowDefinitions.Select(selector: item => (FlowDefinition)item));
        flowDefinitionBrokerMock.Verify(expression: x => x.GetAllFlowDefinitions(ignoreFilters: false), times: Times.Once);
        flowDefinitionBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Workflow.FlowDefinition>()),
times: Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}