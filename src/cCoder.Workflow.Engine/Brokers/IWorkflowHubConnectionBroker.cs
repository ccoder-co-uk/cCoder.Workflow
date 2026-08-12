// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Brokers;

internal interface IWorkflowHubConnectionBroker
{
    Task ConnectAsync(string url);

    Task SendAsync(
        string level,
        string message,
        string instanceId);

    ValueTask DisconnectAsync();
}