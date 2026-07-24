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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<WorkflowEvent> entities = new[] { CreateRandomWorkflowEvent() }.AsQueryable();
        workflowEventProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<WorkflowEvent> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        workflowEventProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}