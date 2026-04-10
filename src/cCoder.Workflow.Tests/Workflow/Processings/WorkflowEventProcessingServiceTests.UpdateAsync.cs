using System.Security;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenSecurityChecksPassForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        DataUser admin = TestUsers.WithPrivilege("app_admin", 1);
        DataUser executeAsUser = TestUsers.WithPrivilege("page_read", 1);
        executeAsUser.Id = workflowEvent.ExecuteAs;
        currentUser = admin;

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        flowDefinitionServiceMock
            .Setup(x => x.GetAll())
            .Returns(
                new[]
                {
                    new FlowDefinition
                    {
                        Id = workflowEvent.FlowId,
                        AppId = 1,
                        Name = "Flow",
                    },
                }.AsQueryable()
            );

        userBrokerMock.Setup(x => x.GetAllUsers(false)).Returns(new[] { executeAsUser }.AsQueryable());
        workflowEventServiceMock.Setup(x => x.UpdateAsync(workflowEvent)).ReturnsAsync(workflowEvent);

        // When
        WorkflowEvent result = await workflowEventProcessingService.UpdateAsync(workflowEvent);

        // Then
        result.Should().BeSameAs(workflowEvent);
        workflowEventServiceMock.Verify(x => x.UpdateAsync(workflowEvent), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        userBrokerMock.Verify(x => x.GetAllUsers(false), Times.Once);
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.IsAdminOfApp(1), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenSecurityChecksFailForUpdateAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();
        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(false);

        flowDefinitionServiceMock
            .Setup(x => x.GetAll())
            .Returns(Enumerable.Empty<FlowDefinition>().AsQueryable());

        // When
        Func<Task> act = async () =>
            await workflowEventProcessingService.UpdateAsync(workflowEvent);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventServiceMock.Verify(x => x.UpdateAsync(It.IsAny<WorkflowEvent>()), Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}






