// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowHubService
{
    Task LogWorkflowHubConnectionAsync(bool isConnected);

    Task JoinWorkflowHubThreadAsync(
        string connectionId,
        string thread);

    Task LeaveWorkflowHubThreadAsync(
        string connectionId,
        string thread);

    Task SendWorkflowHubMessageAsync(
        string level,
        string message,
        string thread);
}