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
    public async Task ShouldDelegateToFoundationServiceWhenSecurityChecksPassForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent))
            .Returns(value: 1);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"));

        workflowEventServiceMock
            .Setup(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent))
            .ReturnsAsync(value: workflowEvent);

        // When
        WorkflowEvent result = await workflowEventProcessingService.AddWorkflowEventAsync(newEntity: workflowEvent);

        // Then
        result.Should()
            .BeSameAs(expected: workflowEvent);

        workflowEventServiceMock.Verify(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent))
            .Returns(value: 1);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await workflowEventProcessingService.AddWorkflowEventAsync(newEntity: workflowEvent);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        workflowEventServiceMock.Verify(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddWorkflowEventAsync(newWorkflowEvent: It.IsAny<WorkflowEvent>()), times: Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}