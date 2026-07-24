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
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression:x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(value:1);
        authorizationBrokerMock
            .Setup(expression:x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"));
        workflowEventServiceMock
            .Setup(expression:x => x.UpdateAsync(workflowEvent))
            .ReturnsAsync(value:workflowEvent);

        WorkflowEvent result = await workflowEventProcessingService.UpdateAsync(entity:workflowEvent);

        result.Should().BeSameAs(expected:workflowEvent);
        workflowEventServiceMock.Verify(expression:x => x.GetAppIdForWorkflowEvent(workflowEvent), times:Times.Once);
        workflowEventServiceMock.Verify(expression:x => x.UpdateAsync(workflowEvent), times:Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression:x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), times:Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForUpdateAsync()
    {
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(expression:x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(value:1);
        authorizationBrokerMock
            .Setup(expression:x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"))
            .Throws(exception:new SecurityException("Access Denied!"));

        Func<Task> act = async () => await workflowEventProcessingService.UpdateAsync(entity:workflowEvent);

        await act.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern:"Access Denied!");
        workflowEventServiceMock.Verify(expression:x => x.GetAppIdForWorkflowEvent(workflowEvent), times:Times.Once);
        workflowEventServiceMock.Verify(expression:x => x.UpdateAsync(It.IsAny<WorkflowEvent>()), times:Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression:x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), times:Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}