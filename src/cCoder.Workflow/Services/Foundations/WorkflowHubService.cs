// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Models;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowHubService(
    IWorkflowHubBroker workflowHubBroker,
    ILoggingBroker loggingBroker)
    : IWorkflowHubService
{
    private static readonly IDictionary<
        string,
        ICollection<WorkflowHubHistoryItem>> History =
            new Dictionary<string, ICollection<WorkflowHubHistoryItem>>();

    private static readonly IDictionary<string, int> UserCounts =
        new Dictionary<string, int>();

    public Task LogWorkflowHubConnectionAsync(bool isConnected) =>
        TryCatch(operation: () =>
        {
            ValidateWorkflowHubConnection(inputs: [isConnected]);

            loggingBroker.LogDebug(
                message: isConnected
                    ? "New Client connected to WorkflowHub"
                    : "Client disconnected from WorkflowHub");

            return Task.CompletedTask;
        });

    public Task JoinWorkflowHubThreadAsync(
        string connectionId,
        string thread) =>
        TryCatch(operation: async () =>
        {
            ValidateWorkflowHubThreadOnJoin(
                inputs: [connectionId, thread]);

            loggingBroker.LogDebug(
                message: "User joining {Thread}",
                args: thread);

            await workflowHubBroker
                .AddConnectionToWorkflowHubGroupAsync(
                    connectionId: connectionId,
                    thread: thread);

            await workflowHubBroker.SendWorkflowHubCallerMessageAsync(
                connectionId: connectionId,
                level: "info",
                message: "Connected to instance " + thread,
                thread: thread);

            await workflowHubBroker.SendWorkflowHubGroupMessageAsync(
                level: "info",
                message: "User Joined",
                thread: thread);

            if (!History.ContainsKey(key: thread))
            {
                History.Add(
                    key: thread,
                    value: new List<WorkflowHubHistoryItem>());
            }

            if (!UserCounts.TryAdd(key: thread, value: 1))
            {
                UserCounts[thread]++;
            }

            foreach (WorkflowHubHistoryItem item in History[thread])
            {
                await workflowHubBroker
                    .SendWorkflowHubCallerMessageAsync(
                        connectionId: connectionId,
                        level: item.Level,
                        message: item.Message,
                        thread: thread);
            }
        });

    public Task LeaveWorkflowHubThreadAsync(
        string connectionId,
        string thread) =>
        TryCatch(operation: async () =>
        {
            ValidateWorkflowHubThreadOnLeave(
                inputs: [connectionId, thread]);

            loggingBroker.LogDebug(
                message: "User leaving {Thread}",
                args: thread);

            await workflowHubBroker
                .RemoveConnectionFromWorkflowHubGroupAsync(
                    connectionId: connectionId,
                    thread: thread);

            await workflowHubBroker.SendWorkflowHubCallerMessageAsync(
                connectionId: connectionId,
                level: "info",
                message: "Stopped listening to messages for " + thread,
                thread: thread);

            await workflowHubBroker.SendWorkflowHubGroupMessageAsync(
                level: "info",
                message: "User Left",
                thread: thread);

            UserCounts[thread]--;

            if (UserCounts[thread] == 0)
            {
                History.Remove(key: thread);
            }
        });

    public Task SendWorkflowHubMessageAsync(
        string level,
        string message,
        string thread) =>
        TryCatch(operation: async () =>
        {
            ValidateWorkflowHubMessageOnSend(
                inputs: [level, message, thread]);

            LogWorkflowHubMessage(
                level: level,
                message: message,
                thread: thread);

            if (!History.ContainsKey(key: thread))
            {
                History.Add(
                    key: thread,
                    value: new List<WorkflowHubHistoryItem>());
            }

            History[thread].Add(
                item: new WorkflowHubHistoryItem
                {
                    Message = message,
                    Level = level
                });

            await workflowHubBroker.SendWorkflowHubGroupMessageAsync(
                level: level,
                message: message,
                thread: thread);
        });

    private void LogWorkflowHubMessage(
        string level,
        string message,
        string thread)
    {
        string formattedMessage = "{Thread}: {Level} {Message}";
        object[] args = [thread, level, message];

        switch (level)
        {
            case "success":
            case "info":
                loggingBroker.LogInformation(
                    message: formattedMessage,
                    args: args);
                break;
            case "debug":
                loggingBroker.LogDebug(
                    message: formattedMessage,
                    args: args);
                break;
            case "warn":
                loggingBroker.LogWarning(
                    message: formattedMessage,
                    args: args);
                break;
            case "error":
                loggingBroker.LogError(
                    message: formattedMessage,
                    args: args);
                break;
        }
    }
}