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

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<FlowInstanceData> entities = new[]
        {
            CreateRandomFlowInstanceData(),
        }.AsQueryable();

        flowInstanceDataServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<FlowInstanceData> result = flowInstanceDataProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        flowInstanceDataServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}