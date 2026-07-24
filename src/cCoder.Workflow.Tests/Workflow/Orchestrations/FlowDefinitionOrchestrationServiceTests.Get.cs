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

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        FlowDefinition entity = CreateRandomFlowDefinition();
        flowDefinitionProcessingServiceMock.Setup(expression: x => x.Get(flowDefinitionId: id)).Returns(value: entity);

        // When
        FlowDefinition result = orchestrationService.Get(flowDefinitionId: id);

        // Then
        result.Should().BeSameAs(expected: entity);
        flowDefinitionProcessingServiceMock.Verify(expression: x => x.Get(flowDefinitionId: id), times: Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}