using System.Security;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
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
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        flowDefinitionServiceMock
            .Setup(x => x.GetAll())
            .Returns(new[] { new FlowDefinition { Id = workflowEvent.FlowId, AppId = 1, Name = "Flow" } }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize(1, "app_admin"));
        authorizationBrokerMock.Setup(x => x.UserBelongsToApp(workflowEvent.ExecuteAs, 1)).Returns(true);
        workflowEventServiceMock.Setup(x => x.AddAsync(workflowEvent)).ReturnsAsync(workflowEvent);

        // When
        WorkflowEvent result = await workflowEventProcessingService.AddAsync(workflowEvent);

        // Then
        result.Should().BeSameAs(workflowEvent);
        workflowEventServiceMock.Verify(x => x.AddAsync(workflowEvent), Times.Once);
        workflowEventServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(1, "app_admin"), Times.Once);
        authorizationBrokerMock.Verify(x => x.UserBelongsToApp(workflowEvent.ExecuteAs, 1), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenSecurityChecksFailForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        flowDefinitionServiceMock
            .Setup(x => x.GetAll())
            .Returns(Enumerable.Empty<FlowDefinition>().AsQueryable());

        // When
        Func<Task> act = async () => await workflowEventProcessingService.AddAsync(workflowEvent);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventServiceMock.Verify(x => x.AddAsync(It.IsAny<WorkflowEvent>()), Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenExecuteUserIsNotInFlowAppForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

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

        authorizationBrokerMock.Setup(x => x.Authorize(1, "app_admin"));
        authorizationBrokerMock.Setup(x => x.UserBelongsToApp(workflowEvent.ExecuteAs, 1)).Returns(false);

        // When
        Func<Task> act = async () => await workflowEventProcessingService.AddAsync(workflowEvent);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventServiceMock.Verify(x => x.AddAsync(It.IsAny<WorkflowEvent>()), Times.Never);
        workflowEventServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.Verify(x => x.GetAll(), Times.Once);
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(1, "app_admin"), Times.Once);
        authorizationBrokerMock.Verify(x => x.UserBelongsToApp(workflowEvent.ExecuteAs, 1), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}











