// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;
using Moq;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowInstanceServiceTests
{
    private readonly Mock<IScriptService> scriptBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<IWorkflowContextBroker> workflowContextBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<IWorkflowHttpClientBroker>
        workflowHttpClientBrokerMock =
            new(behavior: MockBehavior.Strict);

    private FlowInstanceService CreateService() =>
        new(
            scriptService: scriptBrokerMock.Object,
            workflowContextBroker: workflowContextBrokerMock.Object,
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: new JsonBroker(),
            reflectionBroker: new ReflectionBroker());

    private void SetupStateSave(FlowExecution execution) =>
        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.PutJsonAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>(),
                payload: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(
                result: new WorkflowHttpResult
                {
                    IsSuccess = true,
                    StatusCode = 204,
                    Status = "NoContent"
                }));

    private static FlowExecution CreateFlowExecution() =>
        new()
        {
            Request = new(
                api: "https://localhost/",
                token: "token",
                flowId: Guid.NewGuid(),
                instanceId: Guid.NewGuid()),

            Log = (level, message) => Task.CompletedTask
        };

    private static string SerializeFlowInstanceData(
        FlowExecution execution,
        string contextString) =>
        new JsonBroker().Serialize(
            value: new FlowInstanceData
            {
                Id = execution.Request.InstanceId,
                FlowDefinitionId = execution.Request.FlowId,
                ContextString = contextString,
                FlowDefinition = new()
                {
                    Id = execution.Request.FlowId,
                    AppId = 7
                }
            });
}