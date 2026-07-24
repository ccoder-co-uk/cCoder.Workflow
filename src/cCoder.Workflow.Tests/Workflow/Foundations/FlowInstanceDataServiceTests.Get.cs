// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid flowInstanceDataId = Guid.NewGuid();
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(flowInstanceDataId: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false))
            .Returns(value: new[] { flowInstanceData }.AsQueryable());

        // When
        FlowInstanceData result = flowInstanceDataService.Get(flowInstanceDataId: flowInstanceDataId);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: flowInstanceData);

        flowInstanceDataBrokerMock.Verify(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false), times: Times.Once);

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}