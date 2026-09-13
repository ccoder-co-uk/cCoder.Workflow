// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT005, STXFORMAT009
public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenSecurityChecksPassForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.AuthorizeWorkflowEvent(
                workflowEvent: workflowEvent))
            .Returns(value: true);

        workflowEventServiceMock
            .Setup(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent))
            .ReturnsAsync(value: workflowEvent);

        // When
        WorkflowEvent result = await workflowEventProcessingService.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent);

        // Then
        result.Should()
            .BeSameAs(expected: workflowEvent);


        workflowEventServiceMock.Verify(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.AuthorizeWorkflowEvent(
                workflowEvent: workflowEvent))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await workflowEventProcessingService.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        workflowEventServiceMock.Verify(expression: x => x.AuthorizeWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: It.IsAny<WorkflowEvent>()), times: Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT009