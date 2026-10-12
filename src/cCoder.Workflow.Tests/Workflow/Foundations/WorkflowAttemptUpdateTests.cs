// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Brokers.OData;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowAttemptUpdateTests
{
    [Theory]
    [InlineData("Complete")]
    [InlineData("CompletedWithWarnings")]
    [InlineData("Failed")]
    public async Task ShouldAllowOnlyTheOwnerToFinishAndRejectDuplicateBeginThroughApiAsync(string finalState)
    {
        // Given
        Guid owner = Guid.NewGuid();

        Guid attempt = Guid.NewGuid();

        FlowInstanceData current = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), Caller = "original",
            State = "AwaitingExecution", ContextString = "{\"ExecutionAttemptId\":\"" + attempt + "\",\"Flow\":{\"Data\":42}}"
        };

        JsonBroker json = new();

        Mock<IFlowInstanceDataBroker> broker = new();

        Mock<IAuthorizationBroker> authorization = new();

        authorization.Setup(expression: value => value.IsAuthorized(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Returns(value: true);

        broker.Setup(expression: value => value.SelectAllFlowInstanceData())
            .Returns(valueFunction: () => new[] { Copy(instance: current) }.AsQueryable());

        broker.Setup(expression: value => value.SelectFlowInstanceDataByIdAsync(instanceId: current.Id))
            .ReturnsAsync(valueFunction: () => Copy(instance: current));

        broker.Setup(expression: value => value.CompareExchangeFlowInstanceDataAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>()))
            .ReturnsAsync(valueFunction: (WorkflowInstanceExchange exchange) =>
            {
                if (exchange.Expected.State != current.State || exchange.Expected.ContextString != current.ContextString)
                {
                    return 0;

                }

                current = Copy(instance: exchange.Replacement);

                return 1;

            });

        FlowInstanceDataService foundation = new(flowInstanceDataBroker: broker.Object,
            authorizationBroker: authorization.Object, oDataResultBroker: Mock.Of<IODataResultBroker>(), jsonBroker: json);

        FlowInstanceDataOrchestrationService orchestration = new(
            processingService: new FlowInstanceDataProcessingService(service: foundation),
            eventService: Mock.Of<IFlowInstanceDataEventProcessingService>(), loggingBroker: Mock.Of<ILoggingBroker>());

        FlowInstanceDataController controller = new(service: orchestration);

        FlowInstanceData begin = Copy(instance: current);

        WorkflowAttemptContext context = json.ParseAttemptContext(json: begin.ContextString);

        context.ExecutionOwnerId = owner;

        begin.ContextString = json.SerializeAttemptContext(workflowAttemptContext: context);

        begin.State = "Executing";

        // When
        broker.Setup(expression: value => value.CompareExchangeFlowInstanceDataAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>()))
            .ReturnsAsync(value: 0);

        IActionResult racedBegin = await controller.Put(key: current.Id, updatedFlowInstanceData: begin);

        Assert.IsType<ConflictObjectResult>(@object: racedBegin);

        broker.Setup(expression: value => value.CompareExchangeFlowInstanceDataAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>()))
            .ReturnsAsync(valueFunction: (WorkflowInstanceExchange exchange) =>
            {
                current = Copy(instance: exchange.Replacement);

                return 1;

            });

        IActionResult started = await controller.Put(key: current.Id, updatedFlowInstanceData: begin);

        IActionResult duplicate = await controller.Put(key: current.Id, updatedFlowInstanceData: Copy(instance: begin));

        FlowInstanceData wrongOwner = Copy(instance: current);

        context.ExecutionOwnerId = Guid.NewGuid();

        wrongOwner.ContextString = json.SerializeAttemptContext(workflowAttemptContext: context);

        wrongOwner.State = finalState;

        IActionResult rejected = await controller.Put(key: current.Id, updatedFlowInstanceData: wrongOwner);

        context.ExecutionAttemptId = Guid.NewGuid();

        wrongOwner.ContextString = json.SerializeAttemptContext(workflowAttemptContext: context);

        IActionResult staleAttempt = await controller.Put(key: current.Id, updatedFlowInstanceData: wrongOwner);

        Assert.IsType<ConflictObjectResult>(@object: staleAttempt);

        FlowInstanceData finish = Copy(instance: current);

        finish.State = finalState;

        IActionResult finished = await controller.Put(key: current.Id, updatedFlowInstanceData: finish);

        // Then
        Assert.IsType<OkObjectResult>(@object: started);

        Assert.Equal(expected: "WorkflowAttemptConflict", actual: Assert.IsType<ConflictObjectResult>(@object: duplicate).Value);

        Assert.IsType<ConflictObjectResult>(@object: rejected);

        Assert.IsType<OkObjectResult>(@object: finished);

        Assert.Equal(expected: finalState, actual: current.State);

        Assert.Equal(expected: "original", actual: current.Caller);

    }

    [Fact]
    public async Task ShouldExpireAbandonedOwnerPreserveParametersAndRotateAttemptOnExplicitRetryAsync()
    {
        // Given
        Guid oldAttempt = Guid.NewGuid();

        FlowInstanceData current = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), Caller = "original", State = "Executing",
            Start = DateTimeOffset.UtcNow.AddMinutes(minutes: -31),
            ContextString = "{\"ExecutionAttemptId\":\"" + oldAttempt + "\",\"ExecutionOwnerId\":\"" + Guid.NewGuid() + "\",\"Flow\":{\"Start\":{\"Data\":42}}}"
        };

        Mock<IWorkflowInstanceManagementBroker> broker = new();

        JsonBroker json = new();

        broker.Setup(expression: value => value.SelectClaimedInstanceAsync(flowInstanceDataId: current.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: () => Copy(instance: current));

        broker.Setup(expression: value => value.CompareExchangeInstanceAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunction: (WorkflowInstanceExchange exchange, CancellationToken cancellationToken) =>
            {
                if (exchange.Expected.ContextString != current.ContextString || exchange.Expected.State != current.State)
                {
                    return 0;

                }

                current = Copy(instance: exchange.Replacement);

                return 1;

            });

        WorkflowInstanceManagementService service = new(workflowInstanceManagementBroker: broker.Object, jsonBroker: json);

        DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddMinutes(minutes: -30);

        FlowInstanceData oldSnapshot = Copy(instance: current);

        // When
        int expired = await service.FailExpiredFlowInstanceDataAsync(flowInstanceData: oldSnapshot, cutoff: cutoff, cancellationToken: default);

        // Then
        Assert.Equal(expected: 1, actual: expired);

        Assert.Equal(expected: "Failed", actual: current.State);

        Assert.Contains(expectedSubstring: "configured timeout", actualString: current.ContextString);

        Assert.Contains(expectedSubstring: "\"Data\":42", actualString: current.ContextString);

        Assert.Equal(expected: "original", actual: current.Caller);

        // When
        current.State = "Queued";

        FlowInstanceData retried = await service.PrepareQueuedInstanceAsync(instanceId: current.Id, cancellationToken: default);

        bool staleFailure = await service.RecordTerminalFailureAsync(instanceId: current.Id, flowId: current.FlowDefinitionId,
            attemptId: oldAttempt, failureDescription: "stale failure");

        int staleExpiry = await service.FailExpiredFlowInstanceDataAsync(flowInstanceData: oldSnapshot, cutoff: cutoff, cancellationToken: default);

        // Then
        Assert.Equal(expected: "AwaitingExecution", actual: current.State);

        Assert.NotEqual(expected: oldAttempt, actual: json.ParseAttemptContext(json: retried.ContextString).ExecutionAttemptId);

        Assert.Null(@object: json.ParseAttemptContext(json: retried.ContextString).ExecutionOwnerId);

        Assert.False(condition: staleFailure);

        Assert.Equal(expected: 0, actual: staleExpiry);

        Assert.Equal(expected: "original", actual: current.Caller);

        Assert.Contains(expectedSubstring: "\"Data\":42", actualString: current.ContextString);

    }

    private static FlowInstanceData Copy(FlowInstanceData instance) =>
        new()
        {
            Id = instance.Id, FlowDefinitionId = instance.FlowDefinitionId, Caller = instance.Caller,
            State = instance.State, ContextString = instance.ContextString, Start = instance.Start, End = instance.End
        };

}