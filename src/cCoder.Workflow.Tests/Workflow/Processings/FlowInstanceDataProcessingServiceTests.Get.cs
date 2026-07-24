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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        var id = entity.Id;
        flowInstanceDataServiceMock.Setup(expression: x => x.Get(flowInstanceDataId: id))
            .Returns(value: entity);

        // When
        FlowInstanceData result = flowInstanceDataProcessingService.Get(flowInstanceDataId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);
        flowInstanceDataServiceMock.Verify(expression: x => x.Get(flowInstanceDataId: id), times: Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}