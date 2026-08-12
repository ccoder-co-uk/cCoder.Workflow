// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Dependencies;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class WorkflowHubConnectionBroker
    : IWorkflowHubConnectionBroker
{
    private WorkflowHubConnectionDependency connection;

    public async Task ConnectAsync(string url)
    {
        connection = new(url: url);
        await connection.ConnectAsync();
    }

    public Task SendAsync(
        string level,
        string message,
        string instanceId) =>
        connection.SendAsync(
            level: level,
            message: message,
            instanceId: instanceId);

    public ValueTask DisconnectAsync() =>
        connection.DisposeAsync();
}