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

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();
        var id = entity.Id;
        workflowEventServiceMock.Setup(expression: x => x.Get(workflowEventId: id)).Returns(value: entity);

        // When
        WorkflowEvent result = workflowEventProcessingService.Get(workflowEventId: id);

        // Then
        result.Should().BeSameAs(expected: entity);
        workflowEventServiceMock.Verify(expression: x => x.Get(workflowEventId: id), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}