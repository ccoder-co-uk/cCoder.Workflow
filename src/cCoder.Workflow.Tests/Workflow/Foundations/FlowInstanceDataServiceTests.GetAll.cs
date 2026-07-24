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
            .Setup(expression: x => x.SelectAllFlowInstanceData())
            .Returns(value: flowInstanceDataItems);

        // When
        IQueryable<FlowInstanceData> result = flowInstanceDataService.GetAll();

        // Then
        result.Should()
            .BeEquivalentTo(
expectation: flowInstanceDataItems.Select(selector: item => (FlowInstanceData)item)
        );

        flowInstanceDataBrokerMock.Verify(expression: x => x.SelectAllFlowInstanceData(), times: Times.Once);

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<cCoder.Data.Models.Workflow.FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}