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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<WorkflowEvent> entities = new[] { CreateRandomWorkflowEvent() }.AsQueryable();
        workflowEventServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<WorkflowEvent> result = workflowEventProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: entities);
        workflowEventServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

}