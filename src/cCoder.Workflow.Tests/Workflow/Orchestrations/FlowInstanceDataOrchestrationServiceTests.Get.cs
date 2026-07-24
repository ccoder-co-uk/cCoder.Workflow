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


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowInstanceDataOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FlowInstanceData result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}