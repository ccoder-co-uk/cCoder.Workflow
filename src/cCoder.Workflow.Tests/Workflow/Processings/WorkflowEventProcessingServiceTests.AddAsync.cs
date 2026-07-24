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
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent))
            .Returns(value: 1);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"));
        workflowEventServiceMock
            .Setup(expression: x => x.AddAsync(workflowEvent: workflowEvent))
            .ReturnsAsync(value: workflowEvent);

        WorkflowEvent result = await workflowEventProcessingService.AddAsync(entity: workflowEvent);

        result.Should().BeSameAs(expected: workflowEvent);
        workflowEventServiceMock.Verify(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddAsync(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForAddAsync()
    {
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent))
            .Returns(value: 1);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"))
            .Throws(exception: new SecurityException("Access Denied!"));

        Func<Task> act = async () => await workflowEventProcessingService.AddAsync(entity: workflowEvent);

        await act.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        workflowEventServiceMock.Verify(expression: x => x.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent), times: Times.Once);
        workflowEventServiceMock.Verify(expression: x => x.AddAsync(workflowEvent: It.IsAny<WorkflowEvent>()), times: Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(userId: workflowEvent.ExecuteAs, appId: 1, privilege: "app_admin"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}