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

public partial class WorkflowEventOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        WorkflowEvent entity = CreateRandomWorkflowEvent();

        workflowEventProcessingServiceMock.Setup(expression: x => x.Get(workflowEventId: id))
            .Returns(value: entity);

        // When
        WorkflowEvent result = orchestrationService.Get(workflowEventId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        workflowEventProcessingServiceMock.Verify(expression: x => x.Get(workflowEventId: id), times: Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}