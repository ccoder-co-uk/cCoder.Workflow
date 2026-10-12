// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Coordinations;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestCoordinationServiceTests
{
    [Fact]
    public async Task ShouldPersistTerminalFailureWhenRejectedCredentialPreventsInstanceRetrievalAsync()
    {
        // Given
        FlowInstanceData persistedInstance = new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            Caller = "recorded-caller",
            State = "AwaitingExecution",
            ContextString = "{\"Flow\":{},\"Variables\":{\"original\":42}}"
        };

        Mock<IFlowResultService> results = new();

        Mock<IFlowCommunicationService> communication = new();

        Mock<IFlowExecutionOrchestrationService> execution = new();

        execution.Setup(expression: service => service.ExecuteFlowExecutionAsync(
                flowExecution: It.IsAny<FlowExecution>()))
            .Throws(exception: new HttpRequestException(
                message: "Instance retrieval rejected.",
                inner: null,
                statusCode: HttpStatusCode.NotFound));

        results.Setup(expression: service => service.SaveFlowInstanceDataAsync(
                flowInstanceData: It.IsAny<FlowInstanceData>(),
                apiRoot: It.IsAny<string>(),
                authToken: It.IsAny<string>()))
            .Callback<FlowInstanceData, string, string>(action: (instance, api, token) =>
                persistedInstance = instance);

        WorkflowLifecycleOrchestrationService lifecycle = new(
            flowCommunicationService: communication.Object,
            flowResultService: results.Object);

        WorkflowRequestCoordinationService coordinator = new(
            flowExecutionOrchestrationService: execution.Object,
            workflowLifecycleOrchestrationService: lifecycle);

        WorkflowRequest request = new(
            api: "https://trusted.example/Api/",
            token: "expired-test-token",
            flowId: persistedInstance.FlowDefinitionId,
            instanceId: persistedInstance.Id);

        Guid attempt = Guid.NewGuid();

        persistedInstance.ContextString = "{\"ExecutionAttemptId\":\"" + attempt + "\",\"Flow\":{},\"Variables\":{\"original\":42}}";

        request.ExecutionAttemptId = attempt;

        Mock<cCoder.Workflow.Brokers.IWorkflowInstanceManagementBroker> backendBroker = new();

        backendBroker.Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: persistedInstance.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: () => persistedInstance);

        backendBroker.Setup(expression: broker => broker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.IsAny<cCoder.Workflow.Models.WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: (cCoder.Workflow.Models.WorkflowInstanceExchange exchange, CancellationToken token) =>
            {
                persistedInstance = exchange.Replacement;

                return 1;

            });

        cCoder.Workflow.Services.Foundations.WorkflowInstanceManagementService backend = new(
            workflowInstanceManagementBroker: backendBroker.Object, jsonBroker: new cCoder.Workflow.Brokers.JsonBroker());

        // When
        Exception deliveryFailure = await Record.ExceptionAsync(testCode: async () =>
            await coordinator.ExecuteWorkflowRequestAsync(workflowRequest: request));

        Assert.NotNull(@object: deliveryFailure);

        bool recovered = await backend.RecordTerminalFailureAsync(instanceId: persistedInstance.Id,
            flowId: persistedInstance.FlowDefinitionId, attemptId: attempt, failureDescription: "HttpRequestException HTTP 404");

        // Then
        Assert.True(condition: recovered);

        Assert.Equal(expected: "Failed", actual: persistedInstance.State);

        Assert.Contains(expectedSubstring: "ExecutionLog", actualString: persistedInstance.ContextString);

        Assert.Equal(expected: "recorded-caller", actual: persistedInstance.Caller);

    }
}