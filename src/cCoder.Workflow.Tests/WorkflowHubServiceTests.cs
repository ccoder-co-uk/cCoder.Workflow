// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowHubServiceTests
{
    [Fact]
    public async Task WorkflowHubMessage_WhenClientJoins_IsReplayed()
    {
        // Given
        string connectionId = Guid.NewGuid()
            .ToString();

        string thread = Guid.NewGuid()
            .ToString();

        const string level = "info";
        const string message = "message";

        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()));

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()));

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubGroupMessageAsync(
                    level: level,
                    message: message,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.AddConnectionToWorkflowHubGroupAsync(
                    connectionId: connectionId,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubCallerMessageAsync(
                    connectionId: connectionId,
                    level: "info",
                    message: "Connected to instance " + thread,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubGroupMessageAsync(
                    level: "info",
                    message: "User Joined",
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubCallerMessageAsync(
                    connectionId: connectionId,
                    level: level,
                    message: message,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        WorkflowHubService service = new(
            workflowHubBroker: workflowHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        // When
        await service.SendWorkflowHubMessageAsync(
            level: level,
            message: message,
            thread: thread);

        await service.JoinWorkflowHubThreadAsync(
            connectionId: connectionId,
            thread: thread);

        // Then
        workflowHubBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }
}