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
            .Setup(x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(1);
        authorizationBrokerMock
            .Setup(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"));
        workflowEventServiceMock
            .Setup(x => x.UpdateAsync(workflowEvent))
            .ReturnsAsync(workflowEvent);

        WorkflowEvent result = await workflowEventProcessingService.UpdateAsync(workflowEvent);

        result.Should().BeSameAs(workflowEvent);
        workflowEventServiceMock.Verify(x => x.GetAppIdForWorkflowEvent(workflowEvent), Times.Once);
        workflowEventServiceMock.Verify(x => x.UpdateAsync(workflowEvent), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForUpdateAsync()
    {
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(1);
        authorizationBrokerMock
            .Setup(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"))
            .Throws(new SecurityException("Access Denied!"));

        Func<Task> act = async () => await workflowEventProcessingService.UpdateAsync(workflowEvent);

        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventServiceMock.Verify(x => x.GetAppIdForWorkflowEvent(workflowEvent), Times.Once);
        workflowEventServiceMock.Verify(x => x.UpdateAsync(It.IsAny<WorkflowEvent>()), Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}
