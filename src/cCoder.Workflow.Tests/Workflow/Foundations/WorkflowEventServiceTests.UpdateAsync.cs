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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression:x => x.GetCurrentUser()).Returns(value:new User { Id = "test-user" });
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        WorkflowEvent submitted = null;

        workflowEventBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns(value:(int?)7);
        authorizationBrokerMock.Setup(expression:x => x.Authorize((int?)7, "WorkflowEvent_update"));

        workflowEventBrokerMock
            .Setup(expression:x => x.UpdateWorkflowEventAsync(It.IsAny<WorkflowEvent>()))
            .Callback<WorkflowEvent>(action:candidate => submitted = candidate)
            .ReturnsAsync(valueFunction:(WorkflowEvent value) => value);

        // When
        WorkflowEvent result = await workflowEventService.UpdateAsync(workflowEvent:workflowEvent);

        // Then
        result.Should().BeSameAs(expected:workflowEvent);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected:workflowEvent);
        result.Should().NotBeSameAs(unexpected:submitted);
        submitted.Should().BeEquivalentTo(expectation:workflowEvent);
        result.Should().BeEquivalentTo(expectation:workflowEvent);
        workflowEventBrokerMock.Verify(
expression:            x => x.UpdateWorkflowEventAsync(It.IsAny<WorkflowEvent>()),
times:            Times.Once
        );
        workflowEventBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
times:            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "WorkflowEvent_update"),
times:            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns(value:(int?)7);
        authorizationBrokerMock
            .Setup(expression:x => x.Authorize((int?)7, "WorkflowEvent_update"))
            .Throws(exception:new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.UpdateAsync(workflowEvent:workflowEvent);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern:"Access Denied!");
        workflowEventBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
times:            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "WorkflowEvent_update"),
times:            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}