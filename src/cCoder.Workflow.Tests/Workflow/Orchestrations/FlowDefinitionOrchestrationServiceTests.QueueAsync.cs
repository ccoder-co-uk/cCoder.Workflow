using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCreateAndPersistFlowInstanceWhenQueueAsync()
    {
        Guid id = Guid.NewGuid();
        DataUser user = TestUsers.WithPrivilege("flowdefinition_execute", 1);
        Guid queuedId = Guid.NewGuid();
        FlowDefinition flowDefinition = new()
        {
            Id = id,
            AppId = 1,
            App = new App
            {
                Id = 1,
                Domain = "app.local",
                Name = "App"
            },
            DefinitionJson =
                "{\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects\",\"Name\":\"Start\"}],\"Links\":[]}",
            ConfigJson = "{}"
        };

        flowDefinitionProcessingServiceMock.Setup(x => x.Get(id)).Returns(flowDefinition);
        authorizationBrokerMock.Setup(x => x.Authorize(flowDefinition.AppId, "flowdefinition_execute"));
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(user);
        flowInstanceDataProcessingServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await orchestrationService.QueueAsync(id, "{}");

        result.Should().Be(queuedId);
        flowDefinitionProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        flowInstanceDataProcessingServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == user.Id
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
            Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateAndPersistFlowInstanceWhenQueueAsyncByUserId()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString("N");
        Guid queuedId = Guid.NewGuid();
        FlowDefinition flowDefinition = new()
        {
            Id = id,
            AppId = 1,
            App = new App
            {
                Id = 1,
                Domain = "app.local",
                Name = "App"
            },
            DefinitionJson =
                "{\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects\",\"Name\":\"Start\"}],\"Links\":[]}",
            ConfigJson = "{}"
        };

        flowDefinitionProcessingServiceMock.Setup(x => x.Get(id)).Returns(flowDefinition);
        authorizationBrokerMock.Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataProcessingServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await orchestrationService.QueueAsync(id, asUserId, "{}");

        result.Should().Be(queuedId);
        flowDefinitionProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        flowInstanceDataProcessingServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
            Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAuthorizeAndQueueAsyncByUserIdWithoutResolvingUser()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString("N");
        Guid queuedId = Guid.NewGuid();
        FlowDefinition flowDefinition = new()
        {
            Id = id,
            AppId = 1,
            App = new App
            {
                Id = 1,
                Domain = "app.local",
                Name = "App"
            },
            DefinitionJson =
                "{\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects\",\"Name\":\"Start\"}],\"Links\":[]}",
            ConfigJson = "{}"
        };

        flowDefinitionProcessingServiceMock.Setup(x => x.Get(id)).Returns(flowDefinition);
        authorizationBrokerMock.Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataProcessingServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await orchestrationService.QueueAsync(id, asUserId, "{}");

        result.Should().Be(queuedId);
        flowDefinitionProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetUser(It.IsAny<string>()), Times.Never);
        flowInstanceDataProcessingServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
            Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenQueueAsyncByUserIdIsUnauthorized()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString("N");
        FlowDefinition flowDefinition = new()
        {
            Id = id,
            AppId = 1,
            App = new App
            {
                Id = 1,
                Domain = "app.local",
                Name = "App"
            },
            DefinitionJson =
                "{\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects\",\"Name\":\"Start\"}],\"Links\":[]}",
            ConfigJson = "{}"
        };

        flowDefinitionProcessingServiceMock.Setup(x => x.Get(id)).Returns(flowDefinition);
        authorizationBrokerMock
            .Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"))
            .Throws(new System.Security.SecurityException("Access Denied!"));

        Func<Task> action = async () => _ = await orchestrationService.QueueAsync(id, asUserId, "{}");

        await action.Should().ThrowAsync<System.Security.SecurityException>()
            .WithMessage("Access Denied!");
        flowDefinitionProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}
