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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        WorkflowEvent submitted = null;

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "WorkflowEvent_update"));

        workflowEventBrokerMock
            .Setup(x => x.UpdateWorkflowEventAsync(It.IsAny<WorkflowEvent>()))
            .Callback<WorkflowEvent>(candidate => submitted = candidate)
            .ReturnsAsync((WorkflowEvent value) => value);

        // When
        WorkflowEvent result = await workflowEventService.UpdateAsync(workflowEvent);

        // Then
        result.Should().BeSameAs(workflowEvent);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(workflowEvent);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(workflowEvent);
        result.Should().BeEquivalentTo(workflowEvent);
        workflowEventBrokerMock.Verify(
            x => x.UpdateWorkflowEventAsync(It.IsAny<WorkflowEvent>()),
            Times.Once
        );
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_update"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "WorkflowEvent_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.UpdateAsync(workflowEvent);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_update"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}











