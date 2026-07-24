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
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(id: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(x => x.GetAllFlowInstanceData(false))
            .Returns(new[] { flowInstanceData }.AsQueryable());

        // When
        FlowInstanceData result = flowInstanceDataService.Get(flowInstanceDataId);

        // Then
        result.Should().BeEquivalentTo(flowInstanceData);
        flowInstanceDataBrokerMock.Verify(x => x.GetAllFlowInstanceData(false), Times.Once);
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}