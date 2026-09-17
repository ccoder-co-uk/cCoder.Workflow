// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenSecurityChecksPassForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent))
            .Returns(value: true);

        workflowEventServiceMock
            .Setup(expression: x => x.UpdateWorkflowEventAsync(updatedWorkflowEvent: workflowEvent))
            .ReturnsAsync(value: workflowEvent);

        // When
        WorkflowEvent result = await workflowEventProcessingService.UpdateWorkflowEventAsync(updatedWorkflowEvent: workflowEvent);

        // Then
        result.Should()
            .BeSameAs(expected: workflowEvent);


        workflowEventServiceMock.Verify(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.UpdateWorkflowEventAsync(updatedWorkflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await workflowEventProcessingService.UpdateWorkflowEventAsync(updatedWorkflowEvent: workflowEvent);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        workflowEventServiceMock.Verify(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.UpdateWorkflowEventAsync(updatedWorkflowEvent: It.IsAny<WorkflowEvent>()), times: Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }
}