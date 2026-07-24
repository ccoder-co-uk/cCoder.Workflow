// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowDefinitionServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(flowDefinitionId: flowDefinitionId);

        flowDefinitionBrokerMock
            .Setup(expression: x => x.GetAllFlowDefinitions(ignoreFilters: false))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        // When
        FlowDefinition result = flowDefinitionService.Get(flowDefinitionId: flowDefinitionId);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: flowDefinition);

        flowDefinitionBrokerMock.Verify(expression: x => x.GetAllFlowDefinitions(ignoreFilters: false), times: Times.Once);

        flowDefinitionBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowDefinition>()),
times: Times.AtMostOnce()
        );

        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}