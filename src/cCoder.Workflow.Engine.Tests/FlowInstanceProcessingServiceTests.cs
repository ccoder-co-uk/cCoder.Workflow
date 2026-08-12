// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Extensions;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowInstanceProcessingServiceTests
{
    private readonly Mock<IScriptBroker> scriptBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<IWorkflowContextBroker> workflowContextBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<IWorkflowHttpClientBroker>
        workflowHttpClientBrokerMock =
            new(behavior: MockBehavior.Strict);

    private FlowInstanceProcessingService CreateService() =>
        new(
            scriptBroker: scriptBrokerMock.Object,
            workflowContextBroker: workflowContextBrokerMock.Object,
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object);

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
        JsonConvert.SerializeObject(
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
            },
            settings: ObjectExtensions.GetJsonSettings());
}