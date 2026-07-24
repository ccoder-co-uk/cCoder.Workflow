// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
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

        flowDefinitionOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id, asUserId, "{}");

        result.Should().Be(queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        flowInstanceDataOrchestrationServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
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

        flowDefinitionOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id, asUserId, "{}");

        result.Should().Be(queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetUser(It.IsAny<string>()), Times.Never);
        flowInstanceDataOrchestrationServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
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

        flowDefinitionOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"))
            .Throws(new System.Security.SecurityException("Access Denied!"));

        Func<Task> action = async () => _ = await coordinationService.QueueAsync(id, asUserId, "{}");

        await action.Should().ThrowAsync<System.Security.SecurityException>()
            .WithMessage("Access Denied!");
        flowDefinitionOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseUnrestrictedLookupForQueueAsyncByUserId()
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

        flowDefinitionOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id, asUserId, "{}");

        result.Should().Be(queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(x => x.Get(It.IsAny<Guid>()), Times.Never);
        flowDefinitionOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), Times.Once);
        flowInstanceDataOrchestrationServiceMock.Verify(
            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued")),
            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}