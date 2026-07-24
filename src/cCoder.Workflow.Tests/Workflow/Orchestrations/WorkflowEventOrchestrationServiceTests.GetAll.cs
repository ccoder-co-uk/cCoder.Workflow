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
        workflowEventProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true)).Returns(value: entities);

        // When
        IQueryable<WorkflowEvent> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should().BeSameAs(expected: entities);
        workflowEventProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        workflowEventProcessingServiceMock.VerifyNoOtherCalls();
        workflowEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}