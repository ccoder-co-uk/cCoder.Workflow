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
            .Setup(x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(1);
        authorizationBrokerMock
            .Setup(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"));
        workflowEventServiceMock
            .Setup(x => x.AddAsync(workflowEvent))
            .ReturnsAsync(workflowEvent);

        WorkflowEvent result = await workflowEventProcessingService.AddAsync(workflowEvent);

        result.Should().BeSameAs(workflowEvent);
        workflowEventServiceMock.Verify(x => x.GetAppIdForWorkflowEvent(workflowEvent), Times.Once);
        workflowEventServiceMock.Verify(x => x.AddAsync(workflowEvent), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteAsUserIsUnauthorizedForAddAsync()
    {
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventServiceMock
            .Setup(x => x.GetAppIdForWorkflowEvent(workflowEvent))
            .Returns(1);
        authorizationBrokerMock
            .Setup(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"))
            .Throws(new SecurityException("Access Denied!"));

        Func<Task> act = async () => await workflowEventProcessingService.AddAsync(workflowEvent);

        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventServiceMock.Verify(x => x.GetAppIdForWorkflowEvent(workflowEvent), Times.Once);
        workflowEventServiceMock.Verify(x => x.AddAsync(It.IsAny<WorkflowEvent>()), Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(workflowEvent.ExecuteAs, 1, "app_admin"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}
