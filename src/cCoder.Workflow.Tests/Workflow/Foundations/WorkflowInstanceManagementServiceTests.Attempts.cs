// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowInstanceManagementServiceTests
{
    [Fact]
    public async Task ShouldPreserveCallerAndOriginalValuesWhenClaimingAndRecordingTerminalFailureAsync()
    {
        // Given
        FlowInstanceData current = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), Caller = "original-caller",
            State = "Queued", ContextString = "{\"Flow\":{\"Name\":\"original\"},\"Variables\":{\"precise\":0.12345678901234567890123456789,\"huge\":1e1000,\"timestamp\":\"2026-10-12T13:14:15.1234567+05:30\"},\"ExecutionLog\":[{\"Level\":\"Info\",\"Timestamp\":\"2020-01-01T00:00:00+01:00\",\"Message\":\"original\",\"Custom\":17}]}"
        };

        JsonBroker json = new();

        Mock<IWorkflowInstanceManagementBroker> broker = new();

        broker.Setup(expression: value => value.SelectClaimedInstanceAsync(
                flowInstanceDataId: current.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: () => current);

        broker.Setup(expression: value => value.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: (WorkflowInstanceExchange exchange, CancellationToken token) =>
            {
                current = exchange.Replacement;

                return 1;

            });

        WorkflowInstanceManagementService service = new(workflowInstanceManagementBroker: broker.Object, jsonBroker: json);

        // When
        FlowInstanceData claimed = await service.PrepareQueuedInstanceAsync(instanceId: current.Id, cancellationToken: default);

        Guid attempt = json.ParseAttemptContext(json: claimed.ContextString).ExecutionAttemptId.Value;

        bool recorded = await service.RecordTerminalFailureAsync(instanceId: current.Id,
            flowId: current.FlowDefinitionId, attemptId: attempt, failureDescription: "HttpRequestException HTTP 404");

        // Then
        Assert.True(condition: recorded);

        Assert.Equal(expected: "Failed", actual: current.State);

        Assert.Equal(expected: "original-caller", actual: current.Caller);

        Assert.NotNull(@object: current.End);

        using JsonDocument context = JsonDocument.Parse(json: current.ContextString);

        JsonElement variables = context.RootElement.GetProperty(propertyName: "Variables");

        Assert.Equal(expected: "0.12345678901234567890123456789", actual: variables.GetProperty(propertyName: "precise")
            .GetRawText());

        Assert.Equal(expected: "1e1000", actual: variables.GetProperty(propertyName: "huge")
            .GetRawText());

        Assert.Equal(expected: "2026-10-12T13:14:15.1234567+05:30", actual: variables.GetProperty(propertyName: "timestamp")
            .GetString());

        Assert.Equal(expected: 17, actual: context.RootElement.GetProperty(propertyName: "ExecutionLog")[0].GetProperty(propertyName: "Custom")
            .GetInt32());

        Assert.Equal(expected: "original", actual: context.RootElement.GetProperty(propertyName: "Flow")
            .GetProperty(propertyName: "Name")
            .GetString());

        Assert.Contains(expectedSubstring: "HTTP 404", actualString: current.ContextString);

    }

    [Theory]
    [InlineData("Complete", true, true)]
    [InlineData("Failed", true, true)]
    [InlineData("Queued", true, true)]
    [InlineData("Executing", false, true)]
    [InlineData("AwaitingExecution", true, false)]
    public async Task ShouldIgnoreTerminalStaleAndWrongFlowFailuresAsync(string state, bool matchingAttempt, bool matchingFlow)
    {
        // Given
        Guid attempt = Guid.NewGuid();

        FlowInstanceData current = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), State = state,
            ContextString = "{\"ExecutionAttemptId\":\"" + attempt + "\"}"
        };

        Mock<IWorkflowInstanceManagementBroker> broker = new(behavior: MockBehavior.Strict);

        broker.Setup(expression: value => value.SelectClaimedInstanceAsync(
                flowInstanceDataId: current.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: current);

        WorkflowInstanceManagementService service = new(workflowInstanceManagementBroker: broker.Object, jsonBroker: new JsonBroker());

        // When
        bool result = await service.RecordTerminalFailureAsync(instanceId: current.Id,
            flowId: matchingFlow ? current.FlowDefinitionId : Guid.NewGuid(),
            attemptId: matchingAttempt ? attempt : Guid.NewGuid(), failureDescription: "failure");

        // Then
        Assert.False(condition: result);

        broker.Verify(expression: value => value.CompareExchangeInstanceAsync(
            workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()), times: Times.Never);

    }

    [Fact]
    public async Task ShouldFailMalformedContextWithoutChangingItsBytesAsync()
    {
        // Given
        FlowInstanceData current = new() { Id = Guid.NewGuid(), State = "Queued", ContextString = "not JSON original bytes" };

        byte[] original = current.ContextJson;

        Mock<IWorkflowInstanceManagementBroker> broker = new();

        broker.Setup(expression: value => value.SelectClaimedInstanceAsync(
                flowInstanceDataId: current.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: current);

        broker.Setup(expression: value => value.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: (WorkflowInstanceExchange exchange, CancellationToken token) =>
            {
                current = exchange.Replacement;

                return 1;

            });

        WorkflowInstanceManagementService service = new(workflowInstanceManagementBroker: broker.Object, jsonBroker: new JsonBroker());

        // When
        Exception failure = await Record.ExceptionAsync(testCode: async () =>
            await service.PrepareQueuedInstanceAsync(instanceId: current.Id, cancellationToken: default));

        // Then
        Assert.NotNull(@object: failure);

        Assert.Equal(expected: "Failed", actual: current.State);

        Assert.Equal(expected: original, actual: current.ContextJson);

        Assert.NotNull(@object: current.End);

    }
}