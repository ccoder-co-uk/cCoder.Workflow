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
        string asUserId = Guid.NewGuid().ToString(format:"N");
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
            .Setup(expression:x => x.GetAll(true))
            .Returns(value:new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(expression:x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction:(FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id:id, asUserId:asUserId, args:"{}");

        result.Should().Be(expected:queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        authorizationBrokerMock.Verify(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), times:Times.Once);
        flowInstanceDataOrchestrationServiceMock.Verify(
expression:            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
times:            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAuthorizeAndQueueAsyncByUserIdWithoutResolvingUser()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString(format:"N");
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
            .Setup(expression:x => x.GetAll(true))
            .Returns(value:new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(expression:x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction:(FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id:id, asUserId:asUserId, args:"{}");

        result.Should().Be(expected:queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        authorizationBrokerMock.Verify(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), times:Times.Once);
        authorizationBrokerMock.Verify(expression:x => x.GetUser(It.IsAny<string>()), times:Times.Never);
        flowInstanceDataOrchestrationServiceMock.Verify(
expression:            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
times:            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenQueueAsyncByUserIdIsUnauthorized()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString(format:"N");
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
            .Setup(expression:x => x.GetAll(true))
            .Returns(value:new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock
            .Setup(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"))
            .Throws(exception:new System.Security.SecurityException("Access Denied!"));

        Func<Task> action = async () => _ = await coordinationService.QueueAsync(id:id, asUserId:asUserId, args:"{}");

        await action.Should().ThrowAsync<System.Security.SecurityException>()
            .WithMessage(expectedWildcardPattern:"Access Denied!");
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        authorizationBrokerMock.Verify(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), times:Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseUnrestrictedLookupForQueueAsyncByUserId()
    {
        Guid id = Guid.NewGuid();
        string asUserId = Guid.NewGuid().ToString(format:"N");
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
            .Setup(expression:x => x.GetAll(true))
            .Returns(value:new[] { flowDefinition }.AsQueryable());
        authorizationBrokerMock.Setup(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"));
        flowInstanceDataOrchestrationServiceMock
            .Setup(expression:x => x.AddQueuedAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction:(FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(id:id, asUserId:asUserId, args:"{}");

        result.Should().Be(expected:queuedId);
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.Get(It.IsAny<Guid>()), times:Times.Never);
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.GetAll(true), times:Times.Once);
        authorizationBrokerMock.Verify(expression:x => x.Authorize(asUserId, flowDefinition.AppId, "flowdefinition_execute"), times:Times.Once);
        flowInstanceDataOrchestrationServiceMock.Verify(
expression:            x =>
                x.AddQueuedAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued")),
times:            Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}