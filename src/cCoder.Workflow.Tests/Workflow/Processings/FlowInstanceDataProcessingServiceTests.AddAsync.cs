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
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataServiceMock.Setup(expression:x => x.AddAsync(entity)).ReturnsAsync(value:entity);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.AddAsync(entity:entity);

        // Then
        result.Should().BeSameAs(expected:entity);
        flowInstanceDataServiceMock.Verify(expression:x => x.AddAsync(entity), times:Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}