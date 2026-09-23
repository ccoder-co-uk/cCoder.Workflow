// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IWorkflowHubBroker
{
    Task AddConnectionToWorkflowHubGroupAsync(
        string connectionId,
        string thread);

    Task RemoveConnectionFromWorkflowHubGroupAsync(
        string connectionId,
        string thread);

    Task SendWorkflowHubCallerMessageAsync(
        string connectionId,
        string level,
        string message,
        string thread);

    Task SendWorkflowHubGroupMessageAsync(
        string level,
        string message,
        string thread);
}