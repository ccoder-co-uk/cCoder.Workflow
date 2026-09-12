// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Eventing.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public sealed partial class WorkflowInstanceProcessingServiceTests
{
    [Fact]
    public async Task ShouldIgnoreMissingClaimedWorkflowInstanceAsync()
    {
        // Given
        Guid instanceId = Guid.NewGuid();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instanceId);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldMarkClaimedWorkflowInstanceFailedWhenTokenIssueFailsAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();
        Exception exception = new(message: "Token issue failed");

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        tokenManagerMock
            .Setup(expression: manager => manager.IssueTokenAsync(
                userId: instance.Caller,
                tokenUse: TokenUse.WorkflowExecution))
            .Throws(exception: exception);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: exception,
                message: "Flow instance {InstanceId} execution failed.",
                args: instance.Id));

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        tokenManagerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRaiseWorkflowExecuteEventForClaimedInstanceAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        tokenManagerMock
            .Setup(expression: manager => manager.IssueTokenAsync(
                userId: instance.Caller,
                tokenUse: TokenUse.WorkflowExecution))
            .ReturnsAsync(value: new Token { Id = "token" });

        workflowExecutionEventBrokerMock
            .Setup(expression: broker => broker.RaiseWorkflowExecuteEventAsync(
                message: It.Is<EventMessage<WorkflowRequest>>(match: message =>
                    message.AuthInfo.SSOUserId == instance.Caller
                    &&
                    message.Data.InstanceId == instance.Id
                    && message.Data.FlowId == instance.FlowDefinition.Id
                    && message.Data.AuthToken == "token"
                    && message.Data.Api == $"https://{instance.FlowDefinition.App.Domain}:7157/Api/")))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        tokenManagerMock.VerifyAll();
        workflowExecutionEventBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldLogWhenWorkflowEventPublishFailsAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();
        Exception exception = new(message: "Service Bus publish failed");

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        tokenManagerMock
            .Setup(expression: manager => manager.IssueTokenAsync(
                userId: instance.Caller,
                tokenUse: TokenUse.WorkflowExecution))
            .ReturnsAsync(value: new Token { Id = "token" });

        workflowExecutionEventBrokerMock
            .Setup(expression: broker => broker.RaiseWorkflowExecuteEventAsync(
                message: It.IsAny<EventMessage<WorkflowRequest>>()))
            .ThrowsAsync(exception: exception);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: exception,
                message: "Flow instance {InstanceId} execution failed.",
                args: instance.Id));

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        tokenManagerMock.VerifyAll();
        workflowExecutionEventBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }
}