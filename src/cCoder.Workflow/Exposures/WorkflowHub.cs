// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Services.Foundations;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Workflow.Exposures;

internal sealed class WorkflowHub(
    IWorkflowHubService workflowHubService)
    : Hub, ICompositionExposure
{
    public override Task OnConnectedAsync() =>
        workflowHubService.LogWorkflowHubConnectionAsync(
            isConnected: true);

    public override Task OnDisconnectedAsync(Exception exception) =>
        workflowHubService.LogWorkflowHubConnectionAsync(
            isConnected: false);

    public Task Join(string thread) =>
        workflowHubService.JoinWorkflowHubThreadAsync(
            connectionId: Context.ConnectionId,
            thread: thread);

    public Task Leave(string thread) =>
        workflowHubService.LeaveWorkflowHubThreadAsync(
            connectionId: Context.ConnectionId,
            thread: thread);

    public Task ConsoleSend(
        string level,
        string message,
        string thread) =>
        workflowHubService.SendWorkflowHubMessageAsync(
            level: level,
            message: message,
            thread: thread);

    public Task SendTest(string message, string thread) =>
        workflowHubService.SendWorkflowHubMessageAsync(
            level: "test",
            message: message,
            thread: thread);
}