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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(x => x.GetAllWorkflowEvents(false))
            .Returns(new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "WorkflowEvent_delete"));

        workflowEventBrokerMock
            .Setup(
                x =>
                    x.DeleteWorkflowEventAsync(
                        It.Is<WorkflowEvent>(candidate => candidate.Id == workflowEvent.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await workflowEventService.DeleteAsync(workflowEvent.Id);

        // Then
        workflowEventBrokerMock.Verify(x => x.GetAllWorkflowEvents(false), Times.Once);
        workflowEventBrokerMock.Verify(
            x =>
                x.DeleteWorkflowEventAsync(
                    It.Is<WorkflowEvent>(candidate => candidate.Id == workflowEvent.Id)
                ),
            Times.Once
        );
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(x => x.GetAllWorkflowEvents(false))
            .Returns(new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "WorkflowEvent_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.DeleteAsync(workflowEvent.Id);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventBrokerMock.Verify(x => x.GetAllWorkflowEvents(false), Times.Once);
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}