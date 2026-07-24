// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression:x => x.GetCurrentUser()).Returns(value:new User { Id = "test-user" });
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression:x => x.GetAllWorkflowEvents(false))
            .Returns(value:new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns(value:(int?)7);

        workflowEventBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns(value:(int?)7);
        authorizationBrokerMock.Setup(expression:x => x.Authorize((int?)7, "WorkflowEvent_delete"));

        workflowEventBrokerMock
            .Setup(
expression:                x =>
                    x.DeleteWorkflowEventAsync(
                        It.Is<WorkflowEvent>(candidate => candidate.Id == workflowEvent.Id)
                    )
            )
            .ReturnsAsync(value:1);

        // When
        await workflowEventService.DeleteAsync(id:workflowEvent.Id);

        // Then
        workflowEventBrokerMock.Verify(expression:x => x.GetAllWorkflowEvents(false), times:Times.Once);
        workflowEventBrokerMock.Verify(
expression:            x =>
                x.DeleteWorkflowEventAsync(
                    It.Is<WorkflowEvent>(candidate => candidate.Id == workflowEvent.Id)
                ),
times:            Times.Once
        );
        workflowEventBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
times:            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "WorkflowEvent_delete"),
times:            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression:x => x.GetAllWorkflowEvents(false))
            .Returns(value:new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns(value:(int?)7);
        authorizationBrokerMock
            .Setup(expression:x => x.Authorize((int?)7, "WorkflowEvent_delete"))
            .Throws(exception:new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.DeleteAsync(id:workflowEvent.Id);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern:"Access Denied!");
        workflowEventBrokerMock.Verify(expression:x => x.GetAllWorkflowEvents(false), times:Times.Once);
        workflowEventBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
times:            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "WorkflowEvent_delete"),
times:            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}