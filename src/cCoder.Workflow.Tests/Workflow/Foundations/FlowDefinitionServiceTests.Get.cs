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
            .Setup(x => x.GetAllFlowDefinitions(false))
            .Returns(new[] { flowDefinition }.AsQueryable());

        // When
        FlowDefinition result = flowDefinitionService.Get(flowDefinitionId);

        // Then
        result.Should().BeEquivalentTo(flowDefinition);
        flowDefinitionBrokerMock.Verify(x => x.GetAllFlowDefinitions(false), Times.Once);
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}