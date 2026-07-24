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
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression: x => x.SelectAllWorkflowEvents())
            .Returns(value: new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<WorkflowEvent>()))
            .Returns(value: (int?)7);

        workflowEventBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<WorkflowEvent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_delete"));

        workflowEventBrokerMock
            .Setup(
expression: x =>
                    x.DeleteWorkflowEventAsync(
deletedEntity: It.Is<WorkflowEvent>(match: candidate => candidate.Id == workflowEvent.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await workflowEventService.DeleteAsync(workflowEventId: workflowEvent.Id);

        // Then
        workflowEventBrokerMock.Verify(expression: x => x.SelectAllWorkflowEvents(), times: Times.Once);

        workflowEventBrokerMock.Verify(
expression: x =>
                x.DeleteWorkflowEventAsync(
deletedEntity: It.Is<WorkflowEvent>(match: candidate => candidate.Id == workflowEvent.Id)
                ),
times: Times.Once
        );

        workflowEventBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<WorkflowEvent>()),
times: Times.AtMostOnce()
        );

        workflowEventBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression: x => x.SelectAllWorkflowEvents())
            .Returns(value: new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<WorkflowEvent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.DeleteAsync(workflowEventId: workflowEvent.Id);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        workflowEventBrokerMock.Verify(expression: x => x.SelectAllWorkflowEvents(), times: Times.Once);

        workflowEventBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<WorkflowEvent>()),
times: Times.AtMostOnce()
        );

        workflowEventBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}