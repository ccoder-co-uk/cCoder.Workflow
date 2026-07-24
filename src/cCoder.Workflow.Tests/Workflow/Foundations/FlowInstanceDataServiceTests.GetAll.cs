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

public partial class FlowInstanceDataServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        cCoder.Data.Models.Workflow.FlowInstanceData flowInstanceData =
            CreateRandomFlowInstanceData();
        IQueryable<cCoder.Data.Models.Workflow.FlowInstanceData> flowInstanceDataItems = new[]
        {
            flowInstanceData,
        }.AsQueryable();

        flowInstanceDataBrokerMock
            .Setup(x => x.GetAllFlowInstanceData(false))
            .Returns(flowInstanceDataItems);

        // When
        IQueryable<FlowInstanceData> result = flowInstanceDataService.GetAll();

        // Then
        result.Should().BeEquivalentTo(
            flowInstanceDataItems.Select(item => (FlowInstanceData)item)
        );
        flowInstanceDataBrokerMock.Verify(x => x.GetAllFlowInstanceData(false), Times.Once);
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<cCoder.Data.Models.Workflow.FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}