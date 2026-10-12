// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestCoordinationServiceTests
{
    [Theory]
    [InlineData("owned")]
    [InlineData("other")]
    [InlineData("unavailable")]
    [InlineData("uncommitted")]
    [InlineData("terminal")]
    public async Task ShouldReconcileItsCommittedOwnerAfterExecutingClaimResponseIsLostAsync(string scenario)
    {
        // Given
        Guid attempt = Guid.NewGuid();

        FlowInstanceData persisted = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), State = "AwaitingExecution", Caller = "original-caller",
            FlowDefinition = new() { AppId = 7 },
            ContextString = "{\"ExecutionAttemptId\":\"" + attempt + "\",\"Flow\":{\"Activities\":[{\"Data\":{\"Nested\":{\"Value\":42}}}]}}"
        };

        int reads = 0;
        JsonBroker json = new();

        Mock<IWorkflowHttpClientBroker> http = new();

        Mock<IWorkflowRuntimeService> runtime = new();

        http.Setup(expression: broker => broker.GetStringAsync(apiRoot: It.IsAny<string>(), authToken: It.IsAny<string>(), requestUri: It.IsAny<string>()))
            .ReturnsAsync(valueFunction: () =>
            {
                if (++reads > 1 && scenario == "unavailable")
                {
                    throw new InvalidOperationException(message: "Readback unavailable.");
                }

                return json.Serialize(value: persisted);
            });

        http.Setup(expression: broker => broker.PutJsonAsync(apiRoot: It.IsAny<string>(), authToken: It.IsAny<string>(), requestUri: It.IsAny<string>(), payload: It.IsAny<string>()))
            .Callback<string, string, string, string>(action: (api, token, uri, payload) =>
            {
                if (scenario == "uncommitted")
                {
                    return;
                }

                persisted = json.DeserializeWithoutTypeInformation<FlowInstanceData>(value: payload);

                if (scenario == "other")
                {
                    persisted.ContextString = json.AssignExecutionOwner(context: persisted.ContextString, ownerId: Guid.NewGuid());
                }

                if (scenario == "terminal")
                {
                    persisted.State = "Complete";
                }
            })
            .Throws(exception: new HttpRequestException(message: "Committed claim response lost."));

        runtime.Setup(expression: service => service.ExecuteFlowExecutionAsync(flowExecution: It.IsAny<FlowExecution>()))
            .ReturnsAsync(valueFunction: (FlowExecution value) => value);

        FlowExecutionOrchestrationService service = new(
            scriptService: Mock.Of<IScriptService>(),
            flowInstanceDataService: new FlowInstanceDataService(workflowHttpClientBroker: http.Object, jsonBroker: json),
            workflowRuntimeService: runtime.Object, jsonBroker: json);

        FlowExecution execution = new()
        {
            Request = new WorkflowRequest(api: "https://trusted.example/Api/", token: "test-token", flowId: persisted.FlowDefinitionId, instanceId: persisted.Id)
            {
                ExecutionAttemptId = attempt
            },
            Log = (level, message) => Task.CompletedTask
        };

        // When
        Exception failure = await Record.ExceptionAsync(testCode: async () =>
            await service.ExecuteFlowExecutionAsync(flowExecution: execution));

        // Then
        Assert.NotNull(@object: execution.ExecutionOwnerId);

        if (scenario == "owned")
        {
            Assert.Null(@object: failure);
            Assert.Equal(expected: execution.ExecutionOwnerId, actual: json.DeserializeAttemptIdentity(value: persisted.ContextString).ExecutionOwnerId);
            runtime.Verify(expression: value => value.ExecuteFlowExecutionAsync(flowExecution: execution), times: Times.Once);
        }
        else
        {
            Assert.NotNull(@object: failure);
            runtime.Verify(expression: value => value.ExecuteFlowExecutionAsync(flowExecution: execution), times: Times.Never);

            if (scenario == "unavailable")
            {
                Assert.Contains(expectedSubstring: "Committed claim response lost", actualString: failure.ToString());
                Assert.Contains(expectedSubstring: "Readback unavailable", actualString: failure.ToString());
            }
        }

        http.Verify(expression: value => value.GetStringAsync(apiRoot: It.IsAny<string>(), authToken: It.IsAny<string>(), requestUri: It.IsAny<string>()), times: Times.Exactly(callCount: 2));
    }
}