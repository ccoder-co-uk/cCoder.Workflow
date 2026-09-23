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
    public async Task LogConnection_WhenDisconnected_LogsDisconnection()
    {
        // Given
        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                message: "Client disconnected from WorkflowHub",
                args: It.IsAny<object[]>()));

        WorkflowHubService service = new(
            workflowHubBroker: workflowHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        // When
        await service.LogWorkflowHubConnectionAsync(
            isConnected: false);

        // Then
        loggingBrokerMock.VerifyAll();
    }

    [Theory]
    [InlineData("success")]
    [InlineData("debug")]
    [InlineData("warn")]
    [InlineData("error")]
    public async Task SendMessage_WhenLevelIsKnown_LogsAndSends(
        string level)
    {
        // Given
        string thread = Guid.NewGuid()
            .ToString();

        const string message = "message";

        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        SetupMessageLog(
            loggingBrokerMock: loggingBrokerMock,
            level: level,
            thread: thread,
            message: message);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubGroupMessageAsync(
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

        // Then
        workflowHubBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task LeaveThread_WhenLastUserLeaves_RemovesHistory()
    {
        // Given
        string connectionId = Guid.NewGuid()
            .ToString();

        string thread = Guid.NewGuid()
            .ToString();

        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()));

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.AddConnectionToWorkflowHubGroupAsync(
                    connectionId: connectionId,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.RemoveConnectionFromWorkflowHubGroupAsync(
                    connectionId: connectionId,
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubCallerMessageAsync(
                    connectionId: connectionId,
                    level: It.IsAny<string>(),
                    message: It.IsAny<string>(),
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        workflowHubBrokerMock
            .Setup(expression: broker =>
                broker.SendWorkflowHubGroupMessageAsync(
                    level: "info",
                    message: It.IsAny<string>(),
                    thread: thread))
            .Returns(value: Task.CompletedTask);

        WorkflowHubService service = new(
            workflowHubBroker: workflowHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        // When
        await service.JoinWorkflowHubThreadAsync(
            connectionId: connectionId,
            thread: thread);

        await service.LeaveWorkflowHubThreadAsync(
            connectionId: connectionId,
            thread: thread);

        // Then
        workflowHubBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    private static void SetupMessageLog(
        Mock<ILoggingBroker> loggingBrokerMock,
        string level,
        string thread,
        string message)
    {
        const string formattedMessage = "{Thread}: {Level} {Message}";
        object[] expectedArguments = [thread, level, message];

        switch (level)
        {
            case "success":
                loggingBrokerMock
                    .Setup(expression: broker => broker.LogInformation(
                        message: formattedMessage,
                        args: expectedArguments));
                break;
            case "debug":
                loggingBrokerMock
                    .Setup(expression: broker => broker.LogDebug(
                        message: formattedMessage,
                        args: expectedArguments));
                break;
            case "warn":
                loggingBrokerMock
                    .Setup(expression: broker => broker.LogWarning(
                        message: formattedMessage,
                        args: expectedArguments));
                break;
            default:
                loggingBrokerMock
                    .Setup(expression: broker => broker.LogError(
                        message: formattedMessage,
                        args: expectedArguments));
                break;
        }
    }
}