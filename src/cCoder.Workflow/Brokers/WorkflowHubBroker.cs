// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Exposures;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowHubBroker(
    IHubContext<WorkflowHub> workflowHubContext)
    : IWorkflowHubBroker
{
    public Task AddConnectionToWorkflowHubGroupAsync(
        string connectionId,
        string thread) =>
        workflowHubContext.Groups.AddToGroupAsync(
            connectionId: connectionId,
            groupName: thread);

    public Task RemoveConnectionFromWorkflowHubGroupAsync(
        string connectionId,
        string thread) =>
        workflowHubContext.Groups.RemoveFromGroupAsync(
            connectionId: connectionId,
            groupName: thread);

    public Task SendWorkflowHubCallerMessageAsync(
        string connectionId,
        string level,
        string message,
        string thread) =>
        workflowHubContext.Clients.Client(connectionId: connectionId)
            .SendAsync(
                method: "ConsoleReceive",
                arg1: level,
                arg2: message,
                arg3: thread);

    public Task SendWorkflowHubGroupMessageAsync(
        string level,
        string message,
        string thread) =>
        workflowHubContext.Clients.Group(groupName: thread)
            .SendAsync(
                method: "ConsoleReceive",
                arg1: level,
                arg2: message,
                arg3: thread);
}