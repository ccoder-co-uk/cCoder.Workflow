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
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId);

        flowDefinitionBrokerMock
            .Setup(expression:x => x.GetAllFlowDefinitions(false))
            .Returns(value:new[] { flowDefinition }.AsQueryable());

        // When
        FlowDefinition result = flowDefinitionService.Get(id:flowDefinitionId);

        // Then
        result.Should().BeEquivalentTo(expectation:flowDefinition);
        flowDefinitionBrokerMock.Verify(expression:x => x.GetAllFlowDefinitions(false), times:Times.Once);
        flowDefinitionBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<FlowDefinition>()),
times:            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}