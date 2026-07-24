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

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

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
            .Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId))
            .Returns(value: true);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"))
            .Returns(value: new FlowInstanceData
            {
                FlowDefinitionId = id,
                Caller = asUserId,
                State = "Queued",
                ContextString = "{\"ExecutionState\":\"Queued\"}"
            });

        flowInstanceDataOrchestrationServiceMock
            .Setup(expression: x => x.AddQueuedFlowInstanceDataAsync(newEntity: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(flowDefinitionId: id, asUserId: asUserId, args: "{}");

        result.Should()
            .Be(expected: queuedId);

        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId),
            times: Times.Once);

        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"),
            times: Times.Once);

        flowInstanceDataOrchestrationServiceMock.Verify(
expression: x =>
                x.AddQueuedFlowInstanceDataAsync(
newEntity: It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
times: Times.Once);

        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAuthorizeAndQueueAsyncByUserIdWithoutResolvingUser()
    {
        Guid id = Guid.NewGuid();

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

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
            .Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId))
            .Returns(value: true);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"))
            .Returns(value: new FlowInstanceData
            {
                FlowDefinitionId = id,
                Caller = asUserId,
                State = "Queued",
                ContextString = "{\"ExecutionState\":\"Queued\"}"
            });

        flowInstanceDataOrchestrationServiceMock
            .Setup(expression: x => x.AddQueuedFlowInstanceDataAsync(newEntity: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(flowDefinitionId: id, asUserId: asUserId, args: "{}");

        result.Should()
            .Be(expected: queuedId);

        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId),
            times: Times.Once);

        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"),
            times: Times.Once);

        flowInstanceDataOrchestrationServiceMock.Verify(
expression: x =>
                x.AddQueuedFlowInstanceDataAsync(
newEntity: It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\""))),
times: Times.Once);

        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenQueueAsyncByUserIdIsUnauthorized()
    {
        Guid id = Guid.NewGuid();

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

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
            .Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId))
            .Throws(exception: new System.Security.SecurityException("Access Denied!"));

        Func<Task> action = async () => _ = await coordinationService.QueueAsync(flowDefinitionId: id, asUserId: asUserId, args: "{}");

        await action.Should()
            .ThrowAsync<System.Security.SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId),
            times: Times.Once);
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseUnrestrictedLookupForQueueAsyncByUserId()
    {
        Guid id = Guid.NewGuid();

        string asUserId = Guid.NewGuid()
            .ToString(format: "N");

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
            .Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId))
            .Returns(value: true);

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"))
            .Returns(value: new FlowInstanceData
            {
                FlowDefinitionId = id,
                Caller = asUserId,
                State = "Queued",
                ContextString = "{\"ExecutionState\":\"Queued\"}"
            });

        flowInstanceDataOrchestrationServiceMock
            .Setup(expression: x => x.AddQueuedFlowInstanceDataAsync(newEntity: It.IsAny<FlowInstanceData>()))
            .ReturnsAsync(valueFunction: (FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        Guid result = await coordinationService.QueueAsync(flowDefinitionId: id, asUserId: asUserId, args: "{}");

        result.Should()
            .Be(expected: queuedId);

        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.Get(flowDefinitionId: It.IsAny<Guid>()), times: Times.Never);
        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.AuthorizeFlowDefinitionExecution(userId: asUserId, appId: flowDefinition.AppId),
            times: Times.Once);

        flowDefinitionOrchestrationServiceMock.Verify(
            expression: x => x.CreateFlowDefinitionQueuedFlowInstanceData(flowDefinition, asUserId, "{}"),
            times: Times.Once);

        flowInstanceDataOrchestrationServiceMock.Verify(
expression: x =>
                x.AddQueuedFlowInstanceDataAsync(
newEntity: It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == asUserId
                        && instance.State == "Queued")),
times: Times.Once);

        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}